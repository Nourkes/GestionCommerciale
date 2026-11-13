using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Domain.Entities;
using GestionCommerciale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionCommerciale.Infrastructure.Repositories;

public class OrderRepository(AppDbContext db) : IOrderRepository
{
    public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.Orders.AsNoTracking()
                 .Include(o => o.Client)
                 .Include(o => o.Lines)
                 .OrderByDescending(o => o.OrderDate)
                 .ToListAsync(cancellationToken);

    public Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        db.Orders.Include(o => o.Client)
                 .Include(o => o.Lines)
                     .ThenInclude(l => l.Product)
                 .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<string?> GetLastOrderNumberWithPrefixAsync(string prefix, CancellationToken cancellationToken = default) =>
        db.Orders.Where(o => o.OrderNumber.StartsWith(prefix))
                 .OrderByDescending(o => o.OrderNumber)
                 .Select(o => o.OrderNumber)
                 .FirstOrDefaultAsync(cancellationToken);

    public void Add(Order order) => db.Orders.Add(order);

    public void Remove(Order order) => db.Orders.Remove(order);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}