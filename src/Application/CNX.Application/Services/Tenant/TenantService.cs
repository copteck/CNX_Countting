using CNX.Application.DTOs.Tenant;
using CNX.Application.Common;
using CNX.Application.Interfaces;
using CNX.Domain.Entities.Tenant;
using CNX.Domain.Enums;
using CNX.Domain.Interfaces.Repositories;

namespace CNX.Application.Services.Tenant;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TenantService(ITenantRepository tenantRepository, IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<TenantDto>>> GetAllTenantsAsync()
    {
        var tenants = await _tenantRepository.GetAllAsync();
        var dtos = tenants.Where(t => !t.IsDeleted).Select(MapToDto).ToList();
        return ApiResponse<List<TenantDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<TenantDto>> GetTenantByIdAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant == null || tenant.IsDeleted)
            return ApiResponse<TenantDto>.FailResponse("Không tìm thấy khách hàng");

        return ApiResponse<TenantDto>.SuccessResponse(MapToDto(tenant));
    }

    public async Task<ApiResponse<TenantDto>> GetTenantBySubdomainAsync(string subdomain)
    {
        var tenant = await _tenantRepository.GetBySubdomainAsync(subdomain);
        if (tenant == null)
            return ApiResponse<TenantDto>.FailResponse("Không tìm thấy khách hàng");

        return ApiResponse<TenantDto>.SuccessResponse(MapToDto(tenant));
    }

    public async Task<ApiResponse<TenantDto>> CreateTenantAsync(CreateTenantDto dto)
    {
        // Check duplicate subdomain
        var existing = await _tenantRepository.GetBySubdomainAsync(dto.Subdomain);
        if (existing != null)
            return ApiResponse<TenantDto>.FailResponse("Subdomain đã tồn tại");

        // Check duplicate tax code
        var existingTax = await _tenantRepository.GetByTaxCodeAsync(dto.TaxCode);
        if (existingTax != null)
            return ApiResponse<TenantDto>.FailResponse("Mã số thuế đã tồn tại");

        var tenant = new TenantInfo
        {
            CompanyName = dto.CompanyName,
            TaxCode = dto.TaxCode,
            Subdomain = dto.Subdomain.ToLower(),
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            Representative = dto.Representative,
            BusinessType = dto.BusinessType,
            ContractStartDate = dto.ContractStartDate,
            ContractEndDate = dto.ContractEndDate,
            Status = TenantStatus.Active
        };

        await _tenantRepository.AddAsync(tenant);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<TenantDto>.SuccessResponse(MapToDto(tenant), "Tạo khách hàng thành công");
    }

    public async Task<ApiResponse<TenantDto>> UpdateTenantAsync(Guid id, UpdateTenantDto dto)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant == null || tenant.IsDeleted)
            return ApiResponse<TenantDto>.FailResponse("Không tìm thấy khách hàng");

        tenant.CompanyName = dto.CompanyName;
        tenant.Address = dto.Address;
        tenant.Phone = dto.Phone;
        tenant.Email = dto.Email;
        tenant.Representative = dto.Representative;
        tenant.BusinessType = dto.BusinessType;
        tenant.ContractEndDate = dto.ContractEndDate;

        _tenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<TenantDto>.SuccessResponse(MapToDto(tenant), "Cập nhật thành công");
    }

    public async Task<ApiResponse<bool>> DeleteTenantAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant == null)
            return ApiResponse<bool>.FailResponse("Không tìm thấy khách hàng");

        tenant.IsDeleted = true;
        _tenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Xóa khách hàng thành công");
    }

    public async Task<ApiResponse<bool>> ActivateTenantAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant == null)
            return ApiResponse<bool>.FailResponse("Không tìm thấy khách hàng");

        tenant.Status = TenantStatus.Active;
        _tenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Kích hoạt thành công");
    }

    public async Task<ApiResponse<bool>> DeactivateTenantAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant == null)
            return ApiResponse<bool>.FailResponse("Không tìm thấy khách hàng");

        tenant.Status = TenantStatus.Inactive;
        _tenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Vô hiệu hóa thành công");
    }

    private static TenantDto MapToDto(TenantInfo tenant)
    {
        return new TenantDto
        {
            Id = tenant.Id,
            CompanyName = tenant.CompanyName,
            TaxCode = tenant.TaxCode,
            Subdomain = tenant.Subdomain,
            Address = tenant.Address,
            Phone = tenant.Phone,
            Email = tenant.Email,
            Representative = tenant.Representative,
            BusinessType = tenant.BusinessType,
            Status = tenant.Status.ToString(),
            ContractStartDate = tenant.ContractStartDate,
            ContractEndDate = tenant.ContractEndDate,
            LogoUrl = tenant.LogoUrl
        };
    }
}
