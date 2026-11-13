using GestionCommerciale.Application.DTOs.Clients;

namespace GestionCommerciale.Application.Interfaces;

public interface IClientService
{
    Task<List<ClientDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClientDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ClientDto> CreateAsync(SaveClientDto dto, CancellationToken cancellationToken = default);
    Task<ClientDto> UpdateAsync(int id, SaveClientDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}