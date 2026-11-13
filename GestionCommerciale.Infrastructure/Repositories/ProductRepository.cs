using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Domain.Entities;
using GestionCommerciale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionCommerciale.Infrastructure.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.Products.AsNoTracking()
                   .OrderBy(p => p.Name)
                   .ToListAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ReferenceExistsAsync(string reference, int? excludedProductId = null, CancellationToken cancellationToken = default) =>
        db.Products.AnyAsync(p => p.Reference == reference && p.Id != excludedProductId, cancellationToken);

    public Task<bool> IsUsedInOrdersAsync(int productId, CancellationToken cancellationToken = default) =>
        db.OrderLines.AnyAsync(l => l.ProductId == productId, cancellationToken);

    public void Add(Product product) => db.Products.Add(product);

    public void Remove(Product product) => db.Products.Remove(product);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
    public Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default) =>
    db.Products.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
}