using GestionCommerciale.Application.DTOs.Orders;

namespace GestionCommerciale.Application.Interfaces;

public interface IOrderService
{
    Task<List<OrderSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderDetailDto> CreateAsync(SaveOrderDto dto, CancellationToken cancellationToken = default);
    Task<OrderDetailDto> UpdateAsync(int id, SaveOrderDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderDetailDto> ValidateAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderDetailDto> CancelAsync(int id, CancellationToken cancellationToken = default);
}