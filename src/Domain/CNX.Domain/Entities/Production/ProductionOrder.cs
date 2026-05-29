using CNX.Domain.Common;
using CNX.Domain.Enums;

namespace CNX.Domain.Entities.Production;

/// <summary>
/// Lệnh sản xuất
/// </summary>
public class ProductionOrder : BaseEntity
{
    public Guid TenantId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ProductCode { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public string? Unit { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public ProductionOrderStatus Status { get; set; } = ProductionOrderStatus.Planned;
    public string? Notes { get; set; }

    // Navigation
    public ICollection<BillOfMaterial> BillOfMaterials { get; set; } = new List<BillOfMaterial>();
    public ICollection<ProductionCost> Costs { get; set; } = new List<ProductionCost>();
}
