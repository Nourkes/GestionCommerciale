using GestionCommerciale.Domain.Entities;

namespace GestionCommerciale.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ReferenceExistsAsync(string reference, int? excludedProductId = null, CancellationToken cancellationToken = default);
    Task<bool> IsUsedInOrdersAsync(int productId, CancellationToken cancellationToken = default);
    void Add(Product product);
    void Remove(Product product);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}