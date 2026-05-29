using CNX.Domain.Entities.Tenant;
using CNX.Domain.Interfaces.Repositories;
using CNX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CNX.Infrastructure.Repositories;

public class TenantRepository : GenericRepository<TenantInfo>, ITenantRepository
{
    public TenantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TenantInfo?> GetBySubdomainAsync(string subdomain)
    {
        return await _dbSet
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain && !t.IsDeleted);
    }

    public async Task<TenantInfo?> GetByTaxCodeAsync(string taxCode)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.TaxCode == taxCode && !t.IsDeleted);
    }

    public async Task<IEnumerable<TenantInfo>> GetActiveTenantsAsync()
    {
        return await _dbSet
            .Where(t => t.Status == Domain.Enums.TenantStatus.Active && !t.IsDeleted)
            .ToListAsync();
    }
}
