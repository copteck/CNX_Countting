namespace CNX.Application.Interfaces.ExternalApi;

/// <summary>
/// Interface cho việc gọi API đối tác.
/// Hỗ trợ retry, circuit breaker tự động qua Polly.
/// </summary>
public interface IExternalApiClient
{
    /// <summary>
    /// GET request đến API đối tác
    /// </summary>
    Task<TResponse?> GetAsync<TResponse>(string endpoint, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST request đến API đối tác
    /// </summary>
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET request với phân trang - dùng cho data lớn
    /// </summary>
    Task<PagedApiResponse<TResponse>> GetPagedAsync<TResponse>(string endpoint, int page, int pageSize, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Response phân trang từ API đối tác
/// </summary>
public class PagedApiResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage => Page * PageSize < TotalCount;
}
