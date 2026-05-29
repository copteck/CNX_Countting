namespace CNX.StaffPortal.Services;

/// <summary>Một mục menu điều hướng.</summary>
public sealed record MenuItem(string Text, string Icon, string Path);

/// <summary>
/// Cung cấp tra cứu thông tin menu (tiêu đề, icon) theo đường dẫn,
/// phục vụ hiển thị tab control khi điều hướng.
/// </summary>
public sealed class MenuService
{
    /// <summary>Danh sách mục menu của portal nhân viên.</summary>
    public IReadOnlyList<MenuItem> Items { get; } = new List<MenuItem>
    {
        new("Dashboard", "dashboard", "/"),
        new("Khách hàng", "business", "/tenants"),
        new("Kế toán", "account_balance", "/accounting"),
        new("Kho", "inventory_2", "/inventory"),
        new("Sản xuất", "precision_manufacturing", "/production"),
        new("Thuế", "receipt_long", "/tax"),
    };

    /// <summary>Tìm mục menu theo đường dẫn.</summary>
    public MenuItem? Find(string path) => Items.FirstOrDefault(i => i.Path == path);
}
