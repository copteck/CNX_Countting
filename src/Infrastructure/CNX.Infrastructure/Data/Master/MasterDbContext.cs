using CNX.Domain.Entities.Audit;
using CNX.Domain.Entities.ExternalApi;
using CNX.Domain.Entities.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CNX.Infrastructure.Data.Master;

/// <summary>
/// Master Database Context - Chứa thông tin tenant, users, audit log chung.
/// Khi user login, hệ thống lookup từ Master DB để tìm tenant → lấy connection string của tenant DB.
/// </summary>
public class MasterDbContext : IdentityDbContext<ApplicationUser>
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Danh sách tất cả tenant (khách hàng) với connection string riêng
    /// </summary>
    public DbSet<TenantInfo> Tenants { get; set; }

    /// <summary>
    /// Mapping user ↔ tenant
    /// </summary>
    public DbSet<TenantUser> TenantUsers { get; set; }

    /// <summary>
    /// Audit log tổng (admin-level actions)
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; }

    /// <summary>
    /// Trạng thái sync jobs
    /// </summary>
    public DbSet<ApiSyncJob> ApiSyncJobs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TenantInfo>(entity =>
        {
            entity.HasIndex(e => e.Subdomain).IsUnique();
            entity.HasIndex(e => e.TaxCode).IsUnique();
            entity.Property(e => e.CompanyName).HasMaxLength(500);
            entity.Property(e => e.Subdomain).HasMaxLength(100);
            entity.Property(e => e.TaxCode).HasMaxLength(20);
            entity.Property(e => e.DatabaseConnectionString).HasMaxLength(1000);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

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
            entity.HasIndex(e => e.TenantId);
        });

        builder.Entity<ApiSyncJob>(entity =>
        {
            entity.ToTable("ApiSyncJobs");
            entity.Property(e => e.JobName).HasMaxLength(200);
            entity.Property(e => e.PartnerName).HasMaxLength(200);
            entity.Property(e => e.Endpoint).HasMaxLength(2000);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.TenantId);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
