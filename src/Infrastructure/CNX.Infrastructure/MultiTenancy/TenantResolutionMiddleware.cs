using CNX.Domain.Entities.Tenant;
using CNX.Domain.Enums;
using CNX.Infrastructure.Data.Master;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CNX.Infrastructure.MultiTenancy;

/// <summary>
/// Middleware phân giải tenant từ subdomain.
/// Sử dụng MasterDbContext để lookup tenant → lấy connection string của tenant DB.
/// VD: khachhang1.cnxcounting.com -> resolve TenantId + ConnectionString
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, MasterDbContext masterDb)
    {
        var host = context.Request.Host.Host;
        var subdomain = ExtractSubdomain(host);

        // Cũng hỗ trợ header X-Tenant cho development/testing
        if (string.IsNullOrEmpty(subdomain))
        {
            subdomain = context.Request.Headers["X-Tenant"].FirstOrDefault();
        }

        if (!string.IsNullOrEmpty(subdomain) && subdomain != "admin" && subdomain != "www")
        {
            var tenant = await masterDb.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain
                    && t.Status == TenantStatus.Active
                    && !t.IsDeleted);

            if (tenant != null)
            {
                context.Items["TenantId"] = tenant.Id;
                context.Items["TenantInfo"] = tenant;
                context.Items["TenantConnectionString"] = tenant.DatabaseConnectionString;
            }
            else
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Resource not found.");
                return;
            }
        }

        await _next(context);
    }

    private static string? ExtractSubdomain(string host)
    {
        // Remove port if present
        var hostWithoutPort = host.Split(':')[0];
        var parts = hostWithoutPort.Split('.');

        // subdomain.domain.com -> parts[0] is subdomain
        if (parts.Length >= 3)
        {
            return parts[0];
        }

        // For localhost development: use header
        return null;
    }
}
