using CNX.Domain.Interfaces.Repositories;
using CNX.Infrastructure.Data;
using CNX.Infrastructure.MultiTenancy;
using CNX.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CNX.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Multi-tenancy
        services.AddScoped<ITenantAccessor, TenantAccessor>();

        return services;
    }
}
