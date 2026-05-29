using CNX.Domain.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CNX.Infrastructure.Data.Master;

/// <summary>
/// Factory tạo DbContext cho từng tenant dựa trên connection string riêng.
/// Login qua MasterDB → lấy tenant info → tạo TenantDbContext với connection string riêng.
/// </summary>
public interface ITenantDbContextFactory
{
    /// <summary>
    /// Tạo ApplicationDbContext cho tenant cụ thể dựa trên TenantId
    /// </summary>
    Task<ApplicationDbContext> CreateDbContextAsync(Guid tenantId);

    /// <summary>
    /// Tạo ApplicationDbContext cho tenant dựa trên connection string trực tiếp
    /// </summary>
    ApplicationDbContext CreateDbContext(string connectionString);
}

public class TenantDbContextFactory : ITenantDbContextFactory
{
    private readonly MasterDbContext _masterDb;
    private readonly ILogger<TenantDbContextFactory> _logger;

    public TenantDbContextFactory(MasterDbContext masterDb, ILogger<TenantDbContextFactory> logger)
    {
        _masterDb = masterDb;
        _logger = logger;
    }

    public async Task<ApplicationDbContext> CreateDbContextAsync(Guid tenantId)
    {
        var tenant = await _masterDb.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);

        if (tenant == null)
            throw new InvalidOperationException($"Tenant with ID '{tenantId}' not found.");

        if (string.IsNullOrEmpty(tenant.DatabaseConnectionString))
            throw new InvalidOperationException($"Tenant '{tenant.CompanyName}' does not have a database connection string configured.");

        _logger.LogInformation("Creating DbContext for tenant: {TenantName} (ID: {TenantId})", tenant.CompanyName, tenantId);

        return CreateDbContext(tenant.DatabaseConnectionString);
    }

    public ApplicationDbContext CreateDbContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString, b =>
            b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
