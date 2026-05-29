using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CNX.Portal.Api.Controllers;

/// <summary>
/// Base controller cho Portal - tự động lấy TenantId từ middleware
/// </summary>
[ApiController]
[Authorize]
public abstract class BaseTenantController : ControllerBase
{
    protected Guid GetTenantId()
    {
        var tenantId = HttpContext.Items["TenantId"] as Guid?;
        if (!tenantId.HasValue)
            throw new UnauthorizedAccessException("Không xác định được khách hàng");
        return tenantId.Value;
    }
}
