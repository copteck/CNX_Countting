using CNX.Application.DTOs.Tenant;
using CNX.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CNX.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SystemAdmin")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// Lấy danh sách tất cả khách hàng
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _tenantService.GetAllTenantsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin khách hàng theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _tenantService.GetTenantByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Tạo khách hàng mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantDto dto)
    {
        var result = await _tenantService.CreateTenantAsync(dto);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Cập nhật thông tin khách hàng
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantDto dto)
    {
        var result = await _tenantService.UpdateTenantAsync(id, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Xóa khách hàng (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _tenantService.DeleteTenantAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Kích hoạt khách hàng
    /// </summary>
    [HttpPost("{id}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await _tenantService.ActivateTenantAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Vô hiệu hóa khách hàng
    /// </summary>
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _tenantService.DeactivateTenantAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }
}
