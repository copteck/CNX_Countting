using System.Text.Json;
using CNX.Domain.Entities.Audit;
using CNX.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

namespace CNX.Infrastructure.Audit;

/// <summary>
/// EF Core SaveChanges Interceptor - Tự động ghi audit log cho MỌI thay đổi.
/// Ghi: Ai, làm gì, lúc nào, field nào thay đổi, giá trị cũ → mới.
/// </summary>
public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var auditEntries = OnBeforeSaveChanges(eventData.Context);
        if (auditEntries.Count > 0)
        {
            // Store temp audit entries for after save (to get generated IDs)
            eventData.Context.ChangeTracker.AutoDetectChangesEnabled = true;
            _pendingAuditEntries[eventData.Context.ContextId.InstanceId] = auditEntries;
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return await base.SavedChangesAsync(eventData, result, cancellationToken);

        if (_pendingAuditEntries.TryGetValue(eventData.Context.ContextId.InstanceId, out var auditEntries))
        {
            _pendingAuditEntries.Remove(eventData.Context.ContextId.InstanceId);
            await SaveAuditLogsAsync(eventData.Context, auditEntries, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    // Thread-safe storage for pending entries
    private static readonly Dictionary<Guid, List<AuditEntry>> _pendingAuditEntries = new();

    private List<AuditEntry> OnBeforeSaveChanges(DbContext context)
    {
        context.ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
                       ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var tenantId = _httpContextAccessor.HttpContext?.Items["TenantId"] as Guid?;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // Skip AuditLog itself to avoid infinite loop
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry
            {
                EntityName = entry.Entity.GetType().Name,
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                TenantId = tenantId
            };

            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.Action = "Insert";
                    auditEntry.NewValues = GetValues(entry, EntityState.Added);
                    auditEntry.EntityId = GetPrimaryKeyValue(entry);
                    auditEntry.HasTemporaryProperties = entry.Properties.Any(p => p.IsTemporary);
                    if (auditEntry.HasTemporaryProperties)
                        auditEntry.TemporaryEntry = entry;
                    break;

                case EntityState.Modified:
                    auditEntry.Action = "Update";
                    auditEntry.OldValues = GetChangedOldValues(entry);
                    auditEntry.NewValues = GetChangedNewValues(entry);
                    auditEntry.AffectedColumns = GetAffectedColumns(entry);
                    auditEntry.EntityId = GetPrimaryKeyValue(entry);
                    break;

                case EntityState.Deleted:
                    auditEntry.Action = "Delete";
                    auditEntry.OldValues = GetValues(entry, EntityState.Deleted);
                    auditEntry.EntityId = GetPrimaryKeyValue(entry);
                    break;
            }

            auditEntries.Add(auditEntry);
        }

        return auditEntries;
    }

    private async Task SaveAuditLogsAsync(DbContext context, List<AuditEntry> auditEntries, CancellationToken cancellationToken)
    {
        var auditLogs = new List<AuditLog>();

        foreach (var auditEntry in auditEntries)
        {
            // For entries with temporary properties (auto-generated keys), resolve them now
            if (auditEntry.HasTemporaryProperties && auditEntry.TemporaryEntry != null)
            {
                auditEntry.EntityId = GetPrimaryKeyValue(auditEntry.TemporaryEntry);
                auditEntry.NewValues = GetValues(auditEntry.TemporaryEntry, EntityState.Added);
            }

            auditLogs.Add(new AuditLog
            {
                EntityName = auditEntry.EntityName,
                EntityId = auditEntry.EntityId,
                Action = auditEntry.Action,
                UserId = auditEntry.UserId,
                UserName = auditEntry.UserName,
                Timestamp = DateTime.UtcNow,
                OldValues = auditEntry.OldValues,
                NewValues = auditEntry.NewValues,
                AffectedColumns = auditEntry.AffectedColumns,
                TenantId = auditEntry.TenantId,
                IpAddress = auditEntry.IpAddress
            });
        }

        if (auditLogs.Count > 0)
        {
            context.Set<AuditLog>().AddRange(auditLogs);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static string GetPrimaryKeyValue(EntityEntry entry)
    {
        var key = entry.Properties
            .Where(p => p.Metadata.IsPrimaryKey())
            .Select(p => p.CurrentValue?.ToString() ?? "")
            .FirstOrDefault();
        return key ?? "";
    }

    private static string? GetValues(EntityEntry entry, EntityState state)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            if (property.IsTemporary) continue;

            var value = state == EntityState.Added ? property.CurrentValue : property.OriginalValue;
            dict[property.Metadata.Name] = value;
        }

        return dict.Count > 0 ? JsonSerializer.Serialize(dict) : null;
    }

    private static string? GetChangedOldValues(EntityEntry entry)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            if (property.IsModified)
            {
                dict[property.Metadata.Name] = property.OriginalValue;
            }
        }

        return dict.Count > 0 ? JsonSerializer.Serialize(dict) : null;
    }

    private static string? GetChangedNewValues(EntityEntry entry)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            if (property.IsModified)
            {
                dict[property.Metadata.Name] = property.CurrentValue;
            }
        }

        return dict.Count > 0 ? JsonSerializer.Serialize(dict) : null;
    }

    private static string? GetAffectedColumns(EntityEntry entry)
    {
        var columns = entry.Properties
            .Where(p => p.IsModified)
            .Select(p => p.Metadata.Name)
            .ToList();

        return columns.Count > 0 ? string.Join(",", columns) : null;
    }
}

/// <summary>
/// Internal class to hold audit entry data during save
/// </summary>
internal class AuditEntry
{
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public Guid? TenantId { get; set; }
    public string? IpAddress { get; set; }
    public bool HasTemporaryProperties { get; set; }
    public EntityEntry? TemporaryEntry { get; set; }
}
