using CNX.Application.DTOs.Production;
using CNX.Application.Common;

namespace CNX.Application.Interfaces;

public interface IProductionService
{
    Task<ApiResponse<PagedResult<ProductionOrderDto>>> GetProductionOrdersAsync(Guid tenantId, PaginationParams paginationParams);
    Task<ApiResponse<ProductionOrderDto>> GetProductionOrderByIdAsync(Guid tenantId, Guid orderId);
    Task<ApiResponse<ProductionOrderDto>> CreateProductionOrderAsync(Guid tenantId, CreateProductionOrderDto dto);
    Task<ApiResponse<bool>> StartProductionAsync(Guid tenantId, Guid orderId);
    Task<ApiResponse<bool>> CompleteProductionAsync(Guid tenantId, Guid orderId, decimal actualQuantity);
    Task<ApiResponse<bool>> CancelProductionAsync(Guid tenantId, Guid orderId);
}
