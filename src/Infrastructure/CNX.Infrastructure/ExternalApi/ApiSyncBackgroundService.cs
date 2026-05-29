using System.Text.Json;
using System.Threading.Channels;
using CNX.Application.Interfaces.ExternalApi;
using CNX.Domain.Entities.ExternalApi;
using CNX.Infrastructure.Data.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CNX.Infrastructure.ExternalApi;

/// <summary>
/// Background service xử lý batch sync từ API đối tác.
/// Dùng Channel pattern cho queue in-memory, xử lý tuần tự/song song theo cấu hình.
/// Phù hợp cho data "siêu khủng" - xử lý nền không block request.
/// </summary>
public class ApiSyncBackgroundService : BackgroundService
{
    private readonly Channel<Guid> _jobChannel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ApiSyncBackgroundService> _logger;

    public ApiSyncBackgroundService(
        Channel<Guid> jobChannel,
        IServiceScopeFactory scopeFactory,
        ILogger<ApiSyncBackgroundService> logger)
    {
        _jobChannel = jobChannel;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ApiSyncBackgroundService started. Waiting for jobs...");

        await foreach (var jobId in _jobChannel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessJobAsync(jobId, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sync job {JobId}", jobId);
                await UpdateJobStatusAsync(jobId, ApiSyncJobStatus.Failed, ex.Message);
            }
        }
    }

    private async Task ProcessJobAsync(Guid jobId, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var masterDb = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
        var apiClient = scope.ServiceProvider.GetRequiredService<IExternalApiClient>();

        var job = await masterDb.ApiSyncJobs.FindAsync(new object[] { jobId }, cancellationToken);
        if (job == null || job.Status == ApiSyncJobStatus.Cancelled) return;

        _logger.LogInformation("Starting sync job: {JobId}", jobId);

        // Update status to Running
        job.Status = ApiSyncJobStatus.Running;
        job.StartedAt = DateTime.UtcNow;
        await masterDb.SaveChangesAsync(cancellationToken);

        try
        {
            // Batch processing: fetch data in pages
            var page = 1;
            const int pageSize = 500; // Xử lý 500 records/batch
            var hasMore = true;

            while (hasMore && !cancellationToken.IsCancellationRequested)
            {
                // Refresh job from DB to check for cancellation
                await masterDb.Entry(job).ReloadAsync(cancellationToken);
                if (job.Status == ApiSyncJobStatus.Cancelled) break;

                var response = await apiClient.GetPagedAsync<object>(
                    job.Endpoint, page, pageSize,
                    cancellationToken: cancellationToken);

                if (response.Data.Count == 0) break;

                job.TotalRecords = response.TotalCount;
                job.ProcessedRecords += response.Data.Count;
                await masterDb.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Job {JobId}: Processed {Processed}/{Total} records",
                    jobId, job.ProcessedRecords, job.TotalRecords);

                hasMore = response.HasNextPage;
                page++;

                // Throttle: tránh overload API đối tác
                await Task.Delay(100, cancellationToken);
            }

            // Mark as completed
            job.Status = job.FailedRecords > 0
                ? ApiSyncJobStatus.PartiallyCompleted
                : ApiSyncJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            await masterDb.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sync job completed: {JobId}. Processed: {Processed}, Failed: {Failed}",
                jobId, job.ProcessedRecords, job.FailedRecords);
        }
        catch (Exception ex)
        {
            job.Status = ApiSyncJobStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;
            await masterDb.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    private async Task UpdateJobStatusAsync(Guid jobId, ApiSyncJobStatus status, string? errorMessage = null)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var masterDb = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
            var job = await masterDb.ApiSyncJobs.FindAsync(jobId);
            if (job != null)
            {
                job.Status = status;
                job.ErrorMessage = errorMessage;
                job.CompletedAt = DateTime.UtcNow;
                await masterDb.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update job status for {JobId}", jobId);
        }
    }
}

/// <summary>
/// Service để enqueue và quản lý sync jobs
/// </summary>
public class ApiSyncJobService : IApiSyncJobService
{
    private readonly MasterDbContext _masterDb;
    private readonly Channel<Guid> _jobChannel;
    private readonly ILogger<ApiSyncJobService> _logger;

    public ApiSyncJobService(
        MasterDbContext masterDb,
        Channel<Guid> jobChannel,
        ILogger<ApiSyncJobService> logger)
    {
        _masterDb = masterDb;
        _jobChannel = jobChannel;
        _logger = logger;
    }

    public async Task<Guid> EnqueueSyncJobAsync(SyncJobRequest request, CancellationToken cancellationToken = default)
    {
        var job = new ApiSyncJob
        {
            JobName = request.JobName,
            PartnerName = request.PartnerName,
            Endpoint = request.Endpoint,
            TenantId = request.TenantId,
            Status = ApiSyncJobStatus.Pending,
            Configuration = request.Configuration != null
                ? JsonSerializer.Serialize(request.Configuration)
                : null
        };

        _masterDb.ApiSyncJobs.Add(job);
        await _masterDb.SaveChangesAsync(cancellationToken);

        // Enqueue to background processing channel
        await _jobChannel.Writer.WriteAsync(job.Id, cancellationToken);

        _logger.LogInformation("Enqueued sync job: {JobId}", job.Id);
        return job.Id;
    }

    public async Task<SyncJobStatus?> GetJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await _masterDb.ApiSyncJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);

        if (job == null) return null;

        return new SyncJobStatus
        {
            JobId = job.Id,
            Status = job.Status.ToString(),
            TotalRecords = job.TotalRecords,
            ProcessedRecords = job.ProcessedRecords,
            FailedRecords = job.FailedRecords,
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt,
            ErrorMessage = job.ErrorMessage
        };
    }

    public async Task CancelJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await _masterDb.ApiSyncJobs.FindAsync(new object[] { jobId }, cancellationToken);
        if (job != null && job.Status is ApiSyncJobStatus.Pending or ApiSyncJobStatus.Running)
        {
            job.Status = ApiSyncJobStatus.Cancelled;
            job.CompletedAt = DateTime.UtcNow;
            await _masterDb.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Cancelled sync job: {JobId}", jobId);
        }
    }
}
