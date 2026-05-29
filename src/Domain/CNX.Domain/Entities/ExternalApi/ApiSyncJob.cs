using CNX.Domain.Common;

namespace CNX.Domain.Entities.ExternalApi;

/// <summary>
/// Lưu trạng thái các job đồng bộ dữ liệu từ API đối tác
/// </summary>
public class ApiSyncJob : BaseEntity
{
    /// <summary>
    /// Tên job (VD: "SyncInvoicesFromPartnerX")
    /// </summary>
    public string JobName { get; set; } = string.Empty;

    /// <summary>
    /// Tên đối tác/API source
    /// </summary>
    public string PartnerName { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint URL
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái: Pending, Running, Completed, Failed, Cancelled
    /// </summary>
    public ApiSyncJobStatus Status { get; set; } = ApiSyncJobStatus.Pending;

    /// <summary>
    /// Tổng số records cần xử lý
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Số records đã xử lý thành công
    /// </summary>
    public int ProcessedRecords { get; set; }

    /// <summary>
    /// Số records lỗi
    /// </summary>
    public int FailedRecords { get; set; }

    /// <summary>
    /// Thời gian bắt đầu
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Thời gian kết thúc
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Thông báo lỗi (nếu có)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Metadata/Config cho job (JSON)
    /// </summary>
    public string? Configuration { get; set; }

    /// <summary>
    /// TenantId nếu job thuộc về tenant cụ thể
    /// </summary>
    public Guid? TenantId { get; set; }
}

public enum ApiSyncJobStatus
{
    Pending = 0,
    Running = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    PartiallyCompleted = 5
}
