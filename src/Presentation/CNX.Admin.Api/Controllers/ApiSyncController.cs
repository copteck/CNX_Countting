using CNX.Application.Interfaces.ExternalApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CNX.Admin.Api.Controllers;

/// <summary>
/// Controller quản lý sync jobs - gọi API đối tác và xử lý batch data nền.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SystemAdmin")]
public class ApiSyncController : ControllerBase
{
    private readonly IApiSyncJobService _syncJobService;

    public ApiSyncController(IApiSyncJobService syncJobService)
    {
        _syncJobService = syncJobService;
    }

    /// <summary>
    /// Tạo job đồng bộ dữ liệu từ API đối tác (chạy nền)
    /// </summary>
    [HttpPost("jobs")]
    public async Task<IActionResult> CreateSyncJob([FromBody] SyncJobRequest request)
    {
        var jobId = await _syncJobService.EnqueueSyncJobAsync(request);
        return Ok(new
        {
            JobId = jobId,
            Message = "Sync job đã được tạo và đang chạy nền",
            StatusUrl = $"/api/apisync/jobs/{jobId}"
        });
    }

    /// <summary>
    /// Xem trạng thái job
    /// </summary>
    [HttpGet("jobs/{jobId}")]
    public async Task<IActionResult> GetJobStatus(Guid jobId)
    {
        var status = await _syncJobService.GetJobStatusAsync(jobId);
        if (status == null)
            return NotFound(new { Message = "Job không tồn tại" });

        return Ok(status);
    }

    /// <summary>
    /// Cancel job đang chạy
    /// </summary>
    [HttpPost("jobs/{jobId}/cancel")]
    public async Task<IActionResult> CancelJob(Guid jobId)
    {
        await _syncJobService.CancelJobAsync(jobId);
        return Ok(new { Message = "Job đã được cancel" });
    }
}
