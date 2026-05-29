using CNX.Application.Common;
using CNX.Application.DTOs.Accounting;
using CNX.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CNX.Portal.Api.Controllers;

[Route("api/[controller]")]
public class AccountingController : BaseTenantController
{
    private readonly IAccountingService _accountingService;

    public AccountingController(IAccountingService accountingService)
    {
        _accountingService = accountingService;
    }

    /// <summary>
    /// Lấy hệ thống tài khoản
    /// </summary>
    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccountCharts()
    {
        var tenantId = GetTenantId();
        var result = await _accountingService.GetAccountChartsAsync(tenantId);
        return Ok(result);
    }

    /// <summary>
    /// Tạo tài khoản mới
    /// </summary>
    [HttpPost("accounts")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountChartDto dto)
    {
        var tenantId = GetTenantId();
        var result = await _accountingService.CreateAccountAsync(tenantId, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách bút toán
    /// </summary>
    [HttpGet("journal-entries")]
    public async Task<IActionResult> GetJournalEntries([FromQuery] PaginationParams paginationParams)
    {
        var tenantId = GetTenantId();
        var result = await _accountingService.GetJournalEntriesAsync(tenantId, paginationParams);
        return Ok(result);
    }

    /// <summary>
    /// Tạo bút toán mới
    /// </summary>
    [HttpPost("journal-entries")]
    public async Task<IActionResult> CreateJournalEntry([FromBody] CreateJournalEntryDto dto)
    {
        var tenantId = GetTenantId();
        var result = await _accountingService.CreateJournalEntryAsync(tenantId, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Ghi sổ bút toán
    /// </summary>
    [HttpPost("journal-entries/{entryId}/post")]
    public async Task<IActionResult> PostJournalEntry(Guid entryId)
    {
        var tenantId = GetTenantId();
        var result = await _accountingService.PostJournalEntryAsync(tenantId, entryId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách hóa đơn
    /// </summary>
    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices([FromQuery] PaginationParams paginationParams, [FromQuery] bool? isInput)
    {
        var tenantId = GetTenantId();
        var result = await _accountingService.GetInvoicesAsync(tenantId, paginationParams, isInput);
        return Ok(result);
    }

    /// <summary>
    /// Tạo hóa đơn mới
    /// </summary>
    [HttpPost("invoices")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
    {
        var tenantId = GetTenantId();
        var result = await _accountingService.CreateInvoiceAsync(tenantId, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
