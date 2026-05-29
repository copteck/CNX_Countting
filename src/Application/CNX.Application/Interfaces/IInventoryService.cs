using CNX.Application.DTOs.Inventory;
using CNX.Application.Common;

namespace CNX.Application.Interfaces;

public interface IInventoryService
{
    // Warehouses
    Task<ApiResponse<List<WarehouseDto>>> GetWarehousesAsync(Guid tenantId);
    Task<ApiResponse<WarehouseDto>> CreateWarehouseAsync(Guid tenantId, CreateWarehouseDto dto);
    Task<ApiResponse<bool>> DeleteWarehouseAsync(Guid tenantId, Guid warehouseId);

    // Inventory Items
    Task<ApiResponse<PagedResult<InventoryItemDto>>> GetInventoryItemsAsync(Guid tenantId, PaginationParams paginationParams);
    Task<ApiResponse<InventoryItemDto>> CreateInventoryItemAsync(Guid tenantId, CreateInventoryItemDto dto);
    Task<ApiResponse<InventoryItemDto>> GetInventoryItemByIdAsync(Guid tenantId, Guid itemId);

    // Transactions
    Task<ApiResponse<PagedResult<InventoryTransactionDto>>> GetTransactionsAsync(Guid tenantId, PaginationParams paginationParams);
    Task<ApiResponse<InventoryTransactionDto>> CreateTransactionAsync(Guid tenantId, CreateInventoryTransactionDto dto);

    // Reports
    Task<ApiResponse<List<InventoryItemDto>>> GetLowStockItemsAsync(Guid tenantId);
}
