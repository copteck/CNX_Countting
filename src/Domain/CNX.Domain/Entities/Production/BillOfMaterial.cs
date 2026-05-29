using CNX.Domain.Common;

namespace CNX.Domain.Entities.Production;

/// <summary>
/// Định mức nguyên vật liệu (Bill of Materials)
/// </summary>
public class BillOfMaterial : BaseEntity
{
    public Guid ProductionOrderId { get; set; }
    public Guid TenantId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public Guid? InventoryItemId { get; set; }

    // Navigation
    public ProductionOrder ProductionOrder { get; set; } = null!;
}
