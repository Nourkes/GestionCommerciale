using GestionCommerciale.Domain.Entities;

namespace GestionCommerciale.Application.Interfaces;

public interface IClientRepository
{
    Task<List<Client>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, int? excludedClientId = null, CancellationToken cancellationToken = default);
    Task<bool> HasOrdersAsync(int clientId, CancellationToken cancellationToken = default);
    void Add(Client client);
    void Remove(Client client);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}