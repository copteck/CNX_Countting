namespace CNX.Application.DTOs.Tax;

public class TaxReportDto
{
    public Guid Id { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Period { get; set; }
    public bool IsQuarterly { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SubmissionDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? TaxableAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public List<TaxReportDetailDto> Details { get; set; } = new();
}

public class TaxReportDetailDto
{
    public string FieldCode { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Description { get; set; }
}

public class CreateTaxReportDto
{
    public int ReportType { get; set; }
    public int Year { get; set; }
    public int Period { get; set; }
    public bool IsQuarterly { get; set; }
    public DateTime? DueDate { get; set; }
    public List<CreateTaxReportDetailDto> Details { get; set; } = new();
}

public class CreateTaxReportDetailDto
{
    public string FieldCode { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}
