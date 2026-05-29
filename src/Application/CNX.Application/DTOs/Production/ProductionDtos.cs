namespace CNX.Application.DTOs.Production;

public class ProductionOrderDto
{
    public Guid Id { get; set; }
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
    public string Status { get; set; } = string.Empty;
    public List<BillOfMaterialDto> BillOfMaterials { get; set; } = new();
    public decimal TotalCost { get; set; }
}

public class CreateProductionOrderDto
{
    public string ProductName { get; set; } = string.Empty;
    public string? ProductCode { get; set; }
    public decimal PlannedQuantity { get; set; }
    public string? Unit { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public List<CreateBillOfMaterialDto> BillOfMaterials { get; set; } = new();
}

public class BillOfMaterialDto
{
    public Guid Id { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
}

public class CreateBillOfMaterialDto
{
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public Guid? InventoryItemId { get; set; }
}
