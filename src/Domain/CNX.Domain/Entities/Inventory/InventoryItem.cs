using CNX.Domain.Common;

namespace CNX.Domain.Entities.Inventory;

/// <summary>
/// Sản phẩm / Vật tư trong kho
/// </summary>
public class InventoryItem : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid WarehouseId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Unit { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalValue { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? MaxStock { get; set; }
    public string? Barcode { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
}
