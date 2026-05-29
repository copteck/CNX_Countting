using CNX.Application.Interfaces;
using CNX.Application.Services.Tenant;
using Microsoft.Extensions.DependencyInjection;

namespace CNX.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ITenantService, TenantService>();

        // AutoMapper
        // services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
