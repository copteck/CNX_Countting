using System.Threading.Channels;
using CNX.Application.Interfaces.ExternalApi;
using CNX.Domain.Interfaces.Repositories;
using CNX.Infrastructure.Audit;
using CNX.Infrastructure.Data;
using CNX.Infrastructure.Data.Master;
using CNX.Infrastructure.ExternalApi;
using CNX.Infrastructure.MultiTenancy;
using CNX.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace CNX.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ═══════════════════════════════════════════════════════════════
        // FEATURE 1: Database-per-tenant
        // MasterDB chứa thông tin tenant, users, audit chung
        // Mỗi tenant có DB riêng (connection string lưu trong TenantInfo)
        // ═══════════════════════════════════════════════════════════════

        // Master Database (chứa tenant registry, users, sync jobs)
        services.AddDbContext<MasterDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("MasterConnection"),
                b => b.MigrationsAssembly(typeof(MasterDbContext).Assembly.FullName)));

        // Tenant Database - registered as scoped, resolved per-request via TenantDbContextFactory
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var tenantAccessor = sp.GetService<ITenantAccessor>();
            var connectionString = tenantAccessor?.TenantConnectionString
                ?? configuration.GetConnectionString("DefaultConnection");

            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            }
        });

        // Tenant DB Context Factory - cho background jobs, manual resolution
        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Multi-tenancy
        services.AddScoped<ITenantAccessor, TenantAccessor>();

        // ═══════════════════════════════════════════════════════════════
        // FEATURE 2: Automatic Audit Log
        // EF Core SaveChanges Interceptor ghi tự động mọi thay đổi
        // ═══════════════════════════════════════════════════════════════

        services.AddScoped<AuditSaveChangesInterceptor>();

        // ═══════════════════════════════════════════════════════════════
        // FEATURE 3: External API Infrastructure
        // HttpClient + Polly (retry, circuit breaker) + Background job
        // ═══════════════════════════════════════════════════════════════

        // Bind options
        services.Configure<ExternalApiOptions>(configuration.GetSection(ExternalApiOptions.SectionName));

        var apiOptions = configuration.GetSection(ExternalApiOptions.SectionName).Get<ExternalApiOptions>()
            ?? new ExternalApiOptions();

        // HttpClient with Polly retry + circuit breaker
        services.AddHttpClient("ExternalApi", client =>
        {
            if (!string.IsNullOrEmpty(apiOptions.BaseUrl))
                client.BaseAddress = new Uri(apiOptions.BaseUrl);

            client.Timeout = TimeSpan.FromSeconds(apiOptions.TimeoutSeconds);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "CNX-Counting/1.0");
        })
        .AddPolicyHandler(GetRetryPolicy(apiOptions))
        .AddPolicyHandler(GetCircuitBreakerPolicy(apiOptions));

        // External API client
        services.AddScoped<IExternalApiClient, ExternalApiClient>();

        // Background job channel + service
        services.AddSingleton(Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        }));
        services.AddScoped<IApiSyncJobService, ApiSyncJobService>();
        services.AddHostedService<ApiSyncBackgroundService>();

        return services;
    }

    /// <summary>
    /// Polly Retry Policy: Retry N lần với exponential backoff.
    /// Retry khi gặp transient HTTP errors (5xx, 408, network errors).
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ExternalApiOptions options)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: options.RetryCount,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(options.RetryBaseDelaySeconds, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Log sẽ được handle bởi Polly context
                });
    }

    /// <summary>
    /// Polly Circuit Breaker: Ngắt mạch khi API đối tác bị lỗi liên tục.
    /// Tránh gửi request vô nghĩa khi đối tác đang down.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ExternalApiOptions options)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: options.CircuitBreakerThreshold,
                durationOfBreak: TimeSpan.FromSeconds(options.CircuitBreakerDurationSeconds));
    }
}
