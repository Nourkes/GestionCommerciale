using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Domain.Entities;
using GestionCommerciale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionCommerciale.Infrastructure.Repositories;

public class ClientRepository(AppDbContext db) : IClientRepository
{
    public Task<List<Client>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.Clients.AsNoTracking()
                  .OrderBy(c => c.LastName)
                  .ToListAsync(cancellationToken);

    public Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, int? excludedClientId = null, CancellationToken cancellationToken = default) =>
        db.Clients.AnyAsync(c => c.Email == email && c.Id != excludedClientId, cancellationToken);

    public Task<bool> HasOrdersAsync(int clientId, CancellationToken cancellationToken = default) =>
        db.Orders.AnyAsync(o => o.ClientId == clientId, cancellationToken);

    public void Add(Client client) => db.Clients.Add(client);

    public void Remove(Client client) => db.Clients.Remove(client);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}