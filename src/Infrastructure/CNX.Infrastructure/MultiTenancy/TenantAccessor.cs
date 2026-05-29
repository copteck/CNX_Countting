using CNX.Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace CNX.Infrastructure.MultiTenancy;

/// <summary>
/// Xác định TenantId dựa trên subdomain từ HTTP request
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
}
