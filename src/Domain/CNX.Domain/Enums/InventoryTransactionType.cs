namespace CNX.Domain.Enums;

public enum InventoryTransactionType
{
    Import = 1,         // Nhập kho
    Export = 2,         // Xuất kho
    Transfer = 3,       // Chuyển kho
    Adjustment = 4,     // Điều chỉnh
    Return = 5          // Trả hàng
}
