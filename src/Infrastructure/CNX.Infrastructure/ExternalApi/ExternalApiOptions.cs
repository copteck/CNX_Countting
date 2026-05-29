namespace CNX.Infrastructure.ExternalApi;

/// <summary>
/// Cấu hình cho External API clients
/// </summary>
public class ExternalApiOptions
{
    public const string SectionName = "ExternalApi";

    /// <summary>
    /// Base URL mặc định cho API đối tác
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Timeout cho mỗi request (seconds)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Số lần retry khi request thất bại
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Thời gian chờ giữa các lần retry (seconds) - exponential backoff
    /// </summary>
    public int RetryBaseDelaySeconds { get; set; } = 2;

    /// <summary>
    /// Số lần failure liên tiếp trước khi circuit breaker mở
    /// </summary>
    public int CircuitBreakerThreshold { get; set; } = 5;

    /// <summary>
    /// Thời gian circuit breaker mở (seconds) trước khi thử lại
    /// </summary>
    public int CircuitBreakerDurationSeconds { get; set; } = 30;

    /// <summary>
    /// Số lượng concurrent batch jobs tối đa
    /// </summary>
    public int MaxConcurrentJobs { get; set; } = 3;

    /// <summary>
    /// Page size cho batch processing
    /// </summary>
    public int BatchPageSize { get; set; } = 500;
}
