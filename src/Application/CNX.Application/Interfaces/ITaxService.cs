using CNX.Application.DTOs.Tax;
using CNX.Application.Common;

namespace CNX.Application.Interfaces;

public interface ITaxService
{
    Task<ApiResponse<PagedResult<TaxReportDto>>> GetTaxReportsAsync(Guid tenantId, PaginationParams paginationParams);
    Task<ApiResponse<TaxReportDto>> GetTaxReportByIdAsync(Guid tenantId, Guid reportId);
    Task<ApiResponse<TaxReportDto>> CreateTaxReportAsync(Guid tenantId, CreateTaxReportDto dto);
    Task<ApiResponse<bool>> SubmitTaxReportAsync(Guid tenantId, Guid reportId);
    Task<ApiResponse<bool>> ApproveTaxReportAsync(Guid tenantId, Guid reportId);
    Task<ApiResponse<bool>> RejectTaxReportAsync(Guid tenantId, Guid reportId, string reason);
}
