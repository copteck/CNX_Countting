using CNX.Application.Common;
using CNX.Application.DTOs.Production;
using CNX.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CNX.Portal.Api.Controllers;

[Route("api/[controller]")]
public class ProductionController : BaseTenantController
{
    private readonly IProductionService _productionService;

    public ProductionController(IProductionService productionService)
    {
        _productionService = productionService;
    }

    /// <summary>
    /// Lấy danh sách lệnh sản xuất
    /// </summary>
    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders([FromQuery] PaginationParams paginationParams)
    {
        var tenantId = GetTenantId();
        var result = await _productionService.GetProductionOrdersAsync(tenantId, paginationParams);
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết lệnh sản xuất
    /// </summary>
    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrderById(Guid orderId)
    {
        var tenantId = GetTenantId();
        var result = await _productionService.GetProductionOrderByIdAsync(tenantId, orderId);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Tạo lệnh sản xuất mới
    /// </summary>
    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateProductionOrderDto dto)
    {
        var tenantId = GetTenantId();
        var result = await _productionService.CreateProductionOrderAsync(tenantId, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Bắt đầu sản xuất
    /// </summary>
    [HttpPost("orders/{orderId}/start")]
    public async Task<IActionResult> StartProduction(Guid orderId)
    {
        var tenantId = GetTenantId();
        var result = await _productionService.StartProductionAsync(tenantId, orderId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Hoàn thành sản xuất
    /// </summary>
    [HttpPost("orders/{orderId}/complete")]
    public async Task<IActionResult> CompleteProduction(Guid orderId, [FromBody] decimal actualQuantity)
    {
        var tenantId = GetTenantId();
        var result = await _productionService.CompleteProductionAsync(tenantId, orderId, actualQuantity);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Hủy lệnh sản xuất
    /// </summary>
    [HttpPost("orders/{orderId}/cancel")]
    public async Task<IActionResult> CancelProduction(Guid orderId)
    {
        var tenantId = GetTenantId();
        var result = await _productionService.CancelProductionAsync(tenantId, orderId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
