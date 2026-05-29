using CNX.Domain.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CNX.Infrastructure.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<TenantInfo>
{
    public void Configure(EntityTypeBuilder<TenantInfo> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CompanyName).IsRequired().HasMaxLength(500);
        builder.Property(x => x.TaxCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Subdomain).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(20);

        builder.HasIndex(x => x.Subdomain).IsUnique();
        builder.HasIndex(x => x.TaxCode).IsUnique();

        builder.HasMany(x => x.Users)
            .WithOne(x => x.Tenant)
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AccountChartConfiguration : IEntityTypeConfiguration<Domain.Entities.Accounting.AccountChart>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Accounting.AccountChart> builder)
    {
        builder.ToTable("AccountCharts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AccountCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.AccountName).IsRequired().HasMaxLength(500);

        builder.HasIndex(x => new { x.TenantId, x.AccountCode }).IsUnique();

        builder.HasOne(x => x.ParentAccount)
            .WithMany(x => x.ChildAccounts)
            .HasForeignKey(x => x.ParentAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InvoiceConfiguration : IEntityTypeConfiguration<Domain.Entities.Accounting.Invoice>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Accounting.Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.VatAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.VatRate).HasPrecision(5, 2);
        builder.Property(x => x.ExchangeRate).HasPrecision(18, 4);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.Invoice)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class InventoryItemConfiguration : IEntityTypeConfiguration<Domain.Entities.Inventory.InventoryItem>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Inventory.InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ItemCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ItemName).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitCost).HasPrecision(18, 4);
        builder.Property(x => x.TotalValue).HasPrecision(18, 2);

        builder.HasIndex(x => new { x.TenantId, x.ItemCode }).IsUnique();
    }
}
