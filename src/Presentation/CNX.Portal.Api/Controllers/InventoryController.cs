using CNX.Application.Common;
using CNX.Application.DTOs.Inventory;
using CNX.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CNX.Portal.Api.Controllers;

[Route("api/[controller]")]
public class InventoryController : BaseTenantController
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Lấy danh sách kho
    /// </summary>
    [HttpGet("warehouses")]
    public async Task<IActionResult> GetWarehouses()
    {
        var tenantId = GetTenantId();
        var result = await _inventoryService.GetWarehousesAsync(tenantId);
        return Ok(result);
    }

    /// <summary>
    /// Tạo kho mới
    /// </summary>
    [HttpPost("warehouses")]
    public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseDto dto)
    {
        var tenantId = GetTenantId();
        var result = await _inventoryService.CreateWarehouseAsync(tenantId, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách sản phẩm trong kho
    /// </summary>
    [HttpGet("items")]
    public async Task<IActionResult> GetItems([FromQuery] PaginationParams paginationParams)
    {
        var tenantId = GetTenantId();
        var result = await _inventoryService.GetInventoryItemsAsync(tenantId, paginationParams);
        return Ok(result);
    }

    /// <summary>
    /// Tạo sản phẩm mới
    /// </summary>
    [HttpPost("items")]
    public async Task<IActionResult> CreateItem([FromBody] CreateInventoryItemDto dto)
    {
        var tenantId = GetTenantId();
        var result = await _inventoryService.CreateInventoryItemAsync(tenantId, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách phiếu nhập/xuất kho
    /// </summary>
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] PaginationParams paginationParams)
    {
        var tenantId = GetTenantId();
        var result = await _inventoryService.GetTransactionsAsync(tenantId, paginationParams);
        return Ok(result);
    }

    /// <summary>
    /// Tạo phiếu nhập/xuất kho
    /// </summary>
    [HttpPost("transactions")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateInventoryTransactionDto dto)
    {
        var tenantId = GetTenantId();
        var result = await _inventoryService.CreateTransactionAsync(tenantId, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Cảnh báo tồn kho thấp
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockItems()
    {
        var tenantId = GetTenantId();
        var result = await _inventoryService.GetLowStockItemsAsync(tenantId);
        return Ok(result);
    }
}
