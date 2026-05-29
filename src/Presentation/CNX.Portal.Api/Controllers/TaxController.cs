using CNX.Application.Common;
using CNX.Application.DTOs.Tax;
using CNX.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CNX.Portal.Api.Controllers;

[Route("api/[controller]")]
public class TaxController : BaseTenantController
{
    private readonly ITaxService _taxService;

    public TaxController(ITaxService taxService)
    {
        _taxService = taxService;
    }

    /// <summary>
    /// Lấy danh sách báo cáo thuế
    /// </summary>
    [HttpGet("reports")]
    public async Task<IActionResult> GetReports([FromQuery] PaginationParams paginationParams)
    {
        var tenantId = GetTenantId();
        var result = await _taxService.GetTaxReportsAsync(tenantId, paginationParams);
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết báo cáo thuế
    /// </summary>
    [HttpGet("reports/{reportId}")]
    public async Task<IActionResult> GetReportById(Guid reportId)
    {
        var tenantId = GetTenantId();
        var result = await _taxService.GetTaxReportByIdAsync(tenantId, reportId);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Tạo báo cáo thuế mới
    /// </summary>
    [HttpPost("reports")]
    public async Task<IActionResult> CreateReport([FromBody] CreateTaxReportDto dto)
    {
        var tenantId = GetTenantId();
        var result = await _taxService.CreateTaxReportAsync(tenantId, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Nộp báo cáo thuế
    /// </summary>
    [HttpPost("reports/{reportId}/submit")]
    public async Task<IActionResult> SubmitReport(Guid reportId)
    {
        var tenantId = GetTenantId();
        var result = await _taxService.SubmitTaxReportAsync(tenantId, reportId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
