using CNX.Domain.Common;

namespace CNX.Domain.Entities.Production;

/// <summary>
/// Chi phí sản xuất
/// </summary>
public class ProductionCost : BaseEntity
{
    public Guid ProductionOrderId { get; set; }
    public Guid TenantId { get; set; }
    public string CostType { get; set; } = string.Empty; // Nguyên vật liệu, Nhân công, Chi phí chung
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CostDate { get; set; }
    public Guid? AccountId { get; set; }

    // Navigation
    public ProductionOrder ProductionOrder { get; set; } = null!;
}
