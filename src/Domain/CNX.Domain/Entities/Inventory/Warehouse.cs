using CNX.Domain.Common;

namespace CNX.Domain.Entities.Inventory;

/// <summary>
/// Kho hàng
/// </summary>
public class Warehouse : BaseEntity
{
    public Guid TenantId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Manager { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();
}
