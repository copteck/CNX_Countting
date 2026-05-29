using CNX.Domain.Entities.Accounting;
using CNX.Domain.Entities.Audit;
using CNX.Domain.Entities.Inventory;
using CNX.Domain.Entities.Production;
using CNX.Domain.Entities.Tax;
using CNX.Domain.Entities.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CNX.Infrastructure.Data;

/// <summary>
/// Tenant Database Context - Mỗi tenant (khách hàng) có 1 database riêng.
/// Context này được tạo bởi TenantDbContextFactory với connection string riêng cho từng tenant.
/// Chứa toàn bộ data nghiệp vụ: Kế toán, Tồn kho, Sản xuất, Thuế, Audit log...
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

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

    // Audit
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations from assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Soft delete filter - không cần TenantId filter vì mỗi tenant đã có DB riêng
        builder.Entity<AccountChart>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<JournalEntry>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Invoice>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<TaxReport>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Warehouse>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<InventoryItem>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<InventoryTransaction>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<ProductionOrder>().HasQueryFilter(x => !x.IsDeleted);

        // AuditLog configuration
        builder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.EntityName).HasMaxLength(200);
            entity.Property(e => e.EntityId).HasMaxLength(200);
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.UserId).HasMaxLength(200);
            entity.Property(e => e.UserName).HasMaxLength(200);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.HasIndex(e => e.EntityName);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.UserId);
        });
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
    string? TenantConnectionString { get; }
}
