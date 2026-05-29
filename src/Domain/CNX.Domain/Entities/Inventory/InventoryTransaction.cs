using CNX.Domain.Common;
using CNX.Domain.Enums;

namespace CNX.Domain.Entities.Inventory;

/// <summary>
/// Phiếu nhập/xuất kho
/// </summary>
public class InventoryTransaction : BaseEntity
{
    public Guid TenantId { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public Guid InventoryItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public InventoryTransactionType TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public Guid? DestinationWarehouseId { get; set; } // Cho chuyển kho

    // Navigation
    public InventoryItem InventoryItem { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}
