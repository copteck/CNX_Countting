namespace CNX.Application.DTOs.Inventory;

public class WarehouseDto
{
    public Guid Id { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Manager { get; set; }
    public bool IsActive { get; set; }
    public int TotalItems { get; set; }
}

public class CreateWarehouseDto
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Manager { get; set; }
}

public class InventoryItemDto
{
    public Guid Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Unit { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalValue { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
}

public class CreateInventoryItemDto
{
    public Guid WarehouseId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Unit { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? MaxStock { get; set; }
}

public class InventoryTransactionDto
{
    public Guid Id { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? Reference { get; set; }
}

public class CreateInventoryTransactionDto
{
    public Guid InventoryItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public int TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public Guid? DestinationWarehouseId { get; set; }
}
