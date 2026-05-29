using CNX.Domain.Entities.Accounting;
using CNX.Domain.Entities.Inventory;
using CNX.Domain.Entities.Production;
using CNX.Domain.Entities.Tax;
using CNX.Domain.Entities.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CNX.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly Guid? _tenantId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantAccessor? tenantAccessor = null)
        : base(options)
    {
        _tenantId = tenantAccessor?.TenantId;
    }

    // Tenant
    public DbSet<TenantInfo> Tenants { get; set; }
    public DbSet<TenantUser> TenantUsers { get; set; }

    // Accounting
    public DbSet<AccountChart> AccountCharts { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }

    // Tax
    public DbSet<TaxReport> TaxReports { get; set; }
    public DbSet<TaxReportDetail> TaxReportDetails { get; set; }

    // Inventory
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    // Production
    public DbSet<ProductionOrder> ProductionOrders { get; set; }
    public DbSet<BillOfMaterial> BillOfMaterials { get; set; }
    public DbSet<ProductionCost> ProductionCosts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations from assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter for multi-tenancy
        if (_tenantId.HasValue)
        {
            builder.Entity<AccountChart>().HasQueryFilter(x => x.TenantId == _tenantId && !x.IsDeleted);
            builder.Entity<JournalEntry>().HasQueryFilter(x => x.TenantId == _tenantId && !x.IsDeleted);
            builder.Entity<Invoice>().HasQueryFilter(x => x.TenantId == _tenantId && !x.IsDeleted);
            builder.Entity<TaxReport>().HasQueryFilter(x => x.TenantId == _tenantId && !x.IsDeleted);
            builder.Entity<Warehouse>().HasQueryFilter(x => x.TenantId == _tenantId && !x.IsDeleted);
            builder.Entity<InventoryItem>().HasQueryFilter(x => x.TenantId == _tenantId && !x.IsDeleted);
            builder.Entity<InventoryTransaction>().HasQueryFilter(x => x.TenantId == _tenantId && !x.IsDeleted);
            builder.Entity<ProductionOrder>().HasQueryFilter(x => x.TenantId == _tenantId && !x.IsDeleted);
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public bool IsSystemAdmin { get; set; } = false;
}

public interface ITenantAccessor
{
    Guid? TenantId { get; }
}
