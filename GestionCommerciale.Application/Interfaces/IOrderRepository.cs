using GestionCommerciale.Domain.Entities;

namespace GestionCommerciale.Application.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<string?> GetLastOrderNumberWithPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    void Add(Order order);
    void Remove(Order order);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}