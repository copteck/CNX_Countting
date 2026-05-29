using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CNX.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SystemAdmin")]
public class DashboardController : ControllerBase
{
    /// <summary>
    /// Lấy thông tin tổng quan hệ thống
    /// </summary>
    [HttpGet("overview")]
    public IActionResult GetOverview()
    {
        // TODO: Implement real data
        return Ok(new
        {
            TotalTenants = 0,
            ActiveTenants = 0,
            TotalUsers = 0,
            PendingTaxReports = 0,
            SystemHealth = "OK"
        });
    }
}
