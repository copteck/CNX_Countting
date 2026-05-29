namespace CNX.TenantWeb.Services;

/// <summary>M?t m?c menu ?i?u h??ng.</summary>
public sealed record MenuItem(string Text, string Icon, string Path);

/// <summary>
/// Cung c?p tra c?u thông tin menu (tiêu ??, icon) theo ???ng d?n,
/// ph?c v? hi?n th? tab control khi ?i?u h??ng.
/// </summary>
public sealed class MenuService
{
    /// <summary>Danh sách m?c menu c?a c?ng khách hàng.</summary>
    public IReadOnlyList<MenuItem> Items { get; } = new List<MenuItem>
    {
        new("Dashboard", "dashboard", "/"),
        new("K? toán", "account_balance", "/accounting"),
        new("T?n kho", "inventory_2", "/inventory"),
        new("S?n xu?t", "precision_manufacturing", "/production"),
        new("Thu?", "receipt_long", "/tax"),
    };

    /// <summary>Tìm m?c menu theo ???ng d?n.</summary>
    public MenuItem? Find(string path) => Items.FirstOrDefault(i => i.Path == path);
}
