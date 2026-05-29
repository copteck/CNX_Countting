using System.Net.Http.Json;
using System.Text.Json;
using CNX.Application.Interfaces.ExternalApi;
using Microsoft.Extensions.Logging;

namespace CNX.Infrastructure.ExternalApi;

/// <summary>
/// HttpClient wrapper với Polly retry + circuit breaker.
/// Dùng IHttpClientFactory pattern cho connection pooling hiệu quả.
/// </summary>
public class ExternalApiClient : IExternalApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ExternalApiClient> _logger;
    private const string HttpClientName = "ExternalApi";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExternalApiClient(IHttpClientFactory httpClientFactory, ILogger<ExternalApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<TResponse?> GetAsync<TResponse>(
        string endpoint,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient(headers);

        _logger.LogInformation("GET request to: {Endpoint}", endpoint);

        var response = await client.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest data,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient(headers);

        _logger.LogInformation("POST request to: {Endpoint}", endpoint);

        var response = await client.PostAsJsonAsync(endpoint, data, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
    }

    public async Task<PagedApiResponse<TResponse>> GetPagedAsync<TResponse>(
        string endpoint,
        int page,
        int pageSize,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var separator = endpoint.Contains('?') ? "&" : "?";
        var pagedEndpoint = $"{endpoint}{separator}page={page}&pageSize={pageSize}";

        var client = CreateClient(headers);

        _logger.LogInformation("GET paged request to: {Endpoint} (page {Page}, size {PageSize})", endpoint, page, pageSize);

        var response = await client.GetAsync(pagedEndpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PagedApiResponse<TResponse>>(JsonOptions, cancellationToken);
        return result ?? new PagedApiResponse<TResponse>();
    }

    private HttpClient CreateClient(Dictionary<string, string>? headers)
    {
        var client = _httpClientFactory.CreateClient(HttpClientName);

        if (headers != null)
        {
            foreach (var (key, value) in headers)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
            }
        }

        return client;
    }
}
