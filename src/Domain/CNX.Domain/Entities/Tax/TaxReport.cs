using CNX.Domain.Common;
using CNX.Domain.Enums;

namespace CNX.Domain.Entities.Tax;

/// <summary>
/// Báo cáo thuế
/// </summary>
public class TaxReport : BaseEntity
{
    public Guid TenantId { get; set; }
    public TaxReportType ReportType { get; set; }
    public int Year { get; set; }
    public int Period { get; set; } // Tháng hoặc Quý
    public bool IsQuarterly { get; set; } // true = Quý, false = Tháng
    public TaxReportStatus Status { get; set; } = TaxReportStatus.Draft;
    public DateTime? SubmissionDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? TaxableAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? Notes { get; set; }
    public string? FilePath { get; set; }

    // Navigation
    public ICollection<TaxReportDetail> Details { get; set; } = new List<TaxReportDetail>();
}

/// <summary>
/// Chi tiết báo cáo thuế
/// </summary>
public class TaxReportDetail : BaseEntity
{
    public Guid TaxReportId { get; set; }
    public string FieldCode { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }

    // Navigation
    public TaxReport TaxReport { get; set; } = null!;
}
