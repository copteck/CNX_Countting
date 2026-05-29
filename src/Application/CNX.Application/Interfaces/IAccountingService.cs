using CNX.Application.DTOs.Accounting;
using CNX.Application.Common;

namespace CNX.Application.Interfaces;

public interface IAccountingService
{
    // Chart of Accounts
    Task<ApiResponse<List<AccountChartDto>>> GetAccountChartsAsync(Guid tenantId);
    Task<ApiResponse<AccountChartDto>> CreateAccountAsync(Guid tenantId, CreateAccountChartDto dto);
    Task<ApiResponse<bool>> DeleteAccountAsync(Guid tenantId, Guid accountId);

    // Journal Entries
    Task<ApiResponse<PagedResult<JournalEntryDto>>> GetJournalEntriesAsync(Guid tenantId, PaginationParams paginationParams);
    Task<ApiResponse<JournalEntryDto>> CreateJournalEntryAsync(Guid tenantId, CreateJournalEntryDto dto);
    Task<ApiResponse<bool>> PostJournalEntryAsync(Guid tenantId, Guid entryId);

    // Invoices
    Task<ApiResponse<PagedResult<InvoiceDto>>> GetInvoicesAsync(Guid tenantId, PaginationParams paginationParams, bool? isInput = null);
    Task<ApiResponse<InvoiceDto>> CreateInvoiceAsync(Guid tenantId, CreateInvoiceDto dto);
    Task<ApiResponse<InvoiceDto>> GetInvoiceByIdAsync(Guid tenantId, Guid invoiceId);
}
