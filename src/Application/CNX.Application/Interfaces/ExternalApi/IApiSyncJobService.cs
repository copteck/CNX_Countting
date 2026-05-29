namespace CNX.Application.Interfaces.ExternalApi;

/// <summary>
/// Interface cho Background Job Service - xử lý batch data từ API đối tác.
/// Dùng khi data "siêu khủng" cần xử lý nền, không block request.
/// </summary>
public interface IApiSyncJobService
{
    /// <summary>
    /// Tạo và enqueue một sync job mới
    /// </summary>
    Task<Guid> EnqueueSyncJobAsync(SyncJobRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy trạng thái job
    /// </summary>
    Task<SyncJobStatus?> GetJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel job đang chạy
    /// </summary>
    Task CancelJobAsync(Guid jobId, CancellationToken cancellationToken = default);
}

public class SyncJobRequest
{
    public string JobName { get; set; } = string.Empty;
    public string PartnerName { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public Dictionary<string, string>? Configuration { get; set; }
}

public class SyncJobStatus
{
    public Guid JobId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int FailedRecords { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public double ProgressPercentage => TotalRecords > 0 ? (double)ProcessedRecords / TotalRecords * 100 : 0;
}
