using CNX.Domain.Entities.Tenant;

namespace CNX.Domain.Interfaces.Repositories;

public interface ITenantRepository : IGenericRepository<TenantInfo>
{
    Task<TenantInfo?> GetBySubdomainAsync(string subdomain);
    Task<TenantInfo?> GetByTaxCodeAsync(string taxCode);
    Task<IEnumerable<TenantInfo>> GetActiveTenantsAsync();
}
