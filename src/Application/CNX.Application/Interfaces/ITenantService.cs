using CNX.Application.DTOs.Tenant;
using CNX.Application.Common;

namespace CNX.Application.Interfaces;

public interface ITenantService
{
    Task<ApiResponse<List<TenantDto>>> GetAllTenantsAsync();
    Task<ApiResponse<TenantDto>> GetTenantByIdAsync(Guid id);
    Task<ApiResponse<TenantDto>> GetTenantBySubdomainAsync(string subdomain);
    Task<ApiResponse<TenantDto>> CreateTenantAsync(CreateTenantDto dto);
    Task<ApiResponse<TenantDto>> UpdateTenantAsync(Guid id, UpdateTenantDto dto);
    Task<ApiResponse<bool>> DeleteTenantAsync(Guid id);
    Task<ApiResponse<bool>> ActivateTenantAsync(Guid id);
    Task<ApiResponse<bool>> DeactivateTenantAsync(Guid id);
}
