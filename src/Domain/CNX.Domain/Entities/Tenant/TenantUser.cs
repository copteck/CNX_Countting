using CNX.Domain.Common;

namespace CNX.Domain.Entities.Tenant;

/// <summary>
/// Người dùng thuộc về một Tenant
/// </summary>
public class TenantUser : BaseEntity
{
    public Guid TenantId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = "User";
    public bool IsActive { get; set; } = true;

    // Navigation
    public TenantInfo Tenant { get; set; } = null!;
}
