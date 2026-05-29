using CNX.Domain.Entities.Audit;
using CNX.Infrastructure.Data.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNX.Admin.Api.Controllers;

/// <summary>
/// Controller xem Audit Logs - mọi thay đổi dữ liệu trong hệ thống.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SystemAdmin")]
public class AuditLogsController : ControllerBase
{
    private readonly MasterDbContext _masterDb;

    public AuditLogsController(MasterDbContext masterDb)
    {
        _masterDb = masterDb;
    }

    /// <summary>
    /// Lấy danh sách audit logs với phân trang và filter
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string? entityName = null,
        [FromQuery] string? action = null,
        [FromQuery] string? userId = null,
        [FromQuery] Guid? tenantId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _masterDb.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(entityName))
            query = query.Where(a => a.EntityName == entityName);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(a => a.Action == action);

        if (!string.IsNullOrEmpty(userId))
            query = query.Where(a => a.UserId == userId);

        if (tenantId.HasValue)
            query = query.Where(a => a.TenantId == tenantId);

        if (fromDate.HasValue)
            query = query.Where(a => a.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.Timestamp <= toDate.Value);

        var totalCount = await query.CountAsync();
        var logs = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            Data = logs,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        });
    }

    /// <summary>
    /// Xem chi tiết thay đổi của 1 entity cụ thể (history)
    /// </summary>
    [HttpGet("entity/{entityName}/{entityId}")]
    public async Task<IActionResult> GetEntityHistory(string entityName, string entityId)
    {
        var logs = await _masterDb.AuditLogs
            .AsNoTracking()
            .Where(a => a.EntityName == entityName && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

        return Ok(logs);
    }
}
