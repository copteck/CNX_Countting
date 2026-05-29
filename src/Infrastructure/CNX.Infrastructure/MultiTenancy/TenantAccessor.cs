using CNX.Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace CNX.Infrastructure.MultiTenancy;

/// <summary>
/// Xác định TenantId và ConnectionString dựa trên HTTP request.
/// Sau khi TenantResolutionMiddleware phân giải tenant, accessor lấy thông tin từ HttpContext.
/// </summary>
public class TenantAccessor : ITenantAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? TenantId
    {
        get
        {
            var tenantId = _httpContextAccessor.HttpContext?.Items["TenantId"] as Guid?;
            return tenantId;
        }
    }

    public string? TenantConnectionString
    {
        get
        {
            return _httpContextAccessor.HttpContext?.Items["TenantConnectionString"] as string;
        }
    }
}
