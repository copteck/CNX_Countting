using CNX.Domain.Common;

namespace CNX.Domain.Entities.Audit;

/// <summary>
/// Ghi lại mọi thay đổi dữ liệu trong hệ thống.
/// Ai làm gì, lúc nào, field nào thay đổi, giá trị cũ → mới.
/// </summary>
public class AuditLog
{
    public long Id { get; set; }

    /// <summary>
    /// Tên bảng/entity bị thay đổi
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Primary key của record bị thay đổi
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Loại thao tác: Insert, Update, Delete
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// User ID thực hiện thay đổi
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Username/Email thực hiện thay đổi
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Thời điểm thay đổi
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Giá trị cũ (JSON) - chỉ có khi Update/Delete
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// Giá trị mới (JSON) - chỉ có khi Insert/Update
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// Danh sách các field bị thay đổi (comma-separated)
    /// </summary>
    public string? AffectedColumns { get; set; }

    /// <summary>
    /// TenantId (nếu có)
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// IP address của request
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Thông tin thêm (JSON)
    /// </summary>
    public string? AdditionalInfo { get; set; }
}
