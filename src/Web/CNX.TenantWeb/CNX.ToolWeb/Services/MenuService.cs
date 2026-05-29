namespace CNX.ToolWeb.Services;

/// <summary>Một mục menu điều hướng.</summary>
public sealed record MenuItem(string Text, string Icon, string Path);

/// <summary>Một nhóm menu chứa nhiều mục.</summary>
public sealed record MenuGroup(string Title, string Icon, IReadOnlyList<MenuItem> Items);

/// <summary>
/// Cung cấp cấu trúc menu điều hướng cho siêu Tool quản trị.
/// Mỗi nhóm tương ứng một phân hệ nghiệp vụ.
/// </summary>
public sealed class MenuService
{
    /// <summary>Danh sách các nhóm menu của hệ thống quản trị.</summary>
    public IReadOnlyList<MenuGroup> Groups { get; } = new List<MenuGroup>
    {
        new("Tổng quan", "dashboard", new List<MenuItem>
        {
            new("Bảng điều khiển", "space_dashboard", "/"),
            new("Trung tâm điều hành", "hub", "/operations")
        }),
        new("Hóa đơn điện tử", "receipt_long", new List<MenuItem>
        {
            new("Hóa đơn đầu ra", "north_east", "/einvoice/outgoing"),
            new("Hóa đơn đầu vào", "south_west", "/einvoice/incoming"),
            new("Phát hành & ký số", "verified", "/einvoice/issue"),
            new("Tra cứu hóa đơn", "search", "/einvoice/lookup")
        }),
        new("Thương mại điện tử", "storefront", new List<MenuItem>
        {
            new("Quản lý gian hàng", "store", "/ecommerce/shops"),
            new("Đơn hàng online", "shopping_cart", "/ecommerce/orders"),
            new("Đồng bộ sàn TMĐT", "sync", "/ecommerce/sync"),
            new("Khuyến mãi", "local_offer", "/ecommerce/promotions")
        }),
        new("Phần mềm bán hàng POS", "point_of_sale", new List<MenuItem>
        {
            new("Bán hàng tại quầy", "storefront", "/pos/sales"),
            new("Ca làm việc", "schedule", "/pos/shifts"),
            new("Quản lý quầy thu ngân", "payments", "/pos/terminals"),
            new("Báo cáo bán hàng", "insights", "/pos/reports")
        }),
        new("Quản trị SQL", "database", new List<MenuItem>
        {
            new("Trình truy vấn SQL", "terminal", "/sql/query"),
            new("Quản lý cơ sở dữ liệu", "storage", "/sql/databases"),
            new("Sao lưu & phục hồi", "backup", "/sql/backup"),
            new("Giám sát hiệu năng", "monitor_heart", "/sql/monitor")
        }),
        new("Kế toán & Tài chính", "account_balance", new List<MenuItem>
        {
            new("Hệ thống tài khoản", "account_tree", "/accounting/chart"),
            new("Bút toán", "edit_note", "/accounting/journal"),
            new("Sổ cái", "menu_book", "/accounting/ledger"),
            new("Báo cáo tài chính", "assessment", "/accounting/reports")
        }),
        new("Kho & Sản xuất", "inventory", new List<MenuItem>
        {
            new("Quản lý kho", "warehouse", "/inventory/warehouses"),
            new("Sản phẩm & vật tư", "category", "/inventory/items"),
            new("Lệnh sản xuất", "precision_manufacturing", "/production/orders"),
            new("Định mức NVL (BOM)", "list_alt", "/production/bom")
        }),
        new("Báo cáo thuế", "request_quote", new List<MenuItem>
        {
            new("Kê khai GTGT", "percent", "/tax/vat"),
            new("Thuế TNDN / TNCN", "savings", "/tax/income"),
            new("Quy trình duyệt", "fact_check", "/tax/approval")
        }),
        new("Quản trị hệ thống", "admin_panel_settings", new List<MenuItem>
        {
            new("Khách hàng (Tenant)", "groups", "/admin/tenants"),
            new("Người dùng & phân quyền", "manage_accounts", "/admin/users"),
            new("Nhật ký hệ thống", "history", "/admin/audit"),
            new("Cấu hình chung", "settings", "/admin/settings")
        })
    };
}
