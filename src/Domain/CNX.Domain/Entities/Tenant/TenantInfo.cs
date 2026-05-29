using CNX.Domain.Common;
using CNX.Domain.Enums;

namespace CNX.Domain.Entities.Tenant;

/// <summary>
/// Đại diện cho một khách hàng (tenant) - mỗi tenant có subdomain riêng
/// </summary>
public class TenantInfo : BaseEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Representative { get; set; }
    public string? BusinessType { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public DateTime? ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
    public string? DatabaseConnectionString { get; set; }
    public string? LogoUrl { get; set; }

    // Navigation properties
    public ICollection<TenantUser> Users { get; set; } = new List<TenantUser>();
}
