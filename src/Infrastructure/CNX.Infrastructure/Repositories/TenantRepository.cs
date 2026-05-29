using CNX.Domain.Entities.Tenant;
using CNX.Domain.Interfaces.Repositories;
using CNX.Infrastructure.Data.Master;
using Microsoft.EntityFrameworkCore;

namespace CNX.Infrastructure.Repositories;

/// <summary>
/// TenantRepository sử dụng MasterDbContext vì Tenant info nằm trong Master DB.
/// </summary>
public class TenantRepository : ITenantRepository
{
    private readonly MasterDbContext _masterDb;
    private readonly DbSet<TenantInfo> _dbSet;

    public TenantRepository(MasterDbContext masterDb)
    {
        _masterDb = masterDb;
        _dbSet = masterDb.Set<TenantInfo>();
    }

    public async Task<TenantInfo?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TenantInfo>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<TenantInfo>> FindAsync(System.Linq.Expressions.Expression<Func<TenantInfo, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<TenantInfo> AddAsync(TenantInfo entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<TenantInfo> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Update(TenantInfo entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(TenantInfo entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<TenantInfo, bool>>? predicate = null)
    {
        return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
    }

    public async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<TenantInfo, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
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
