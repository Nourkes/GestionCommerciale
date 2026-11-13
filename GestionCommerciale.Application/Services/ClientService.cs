using GestionCommerciale.Application.DTOs.Clients;
using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Application.Mappings;
using GestionCommerciale.Domain.Entities;
using GestionCommerciale.Domain.Exceptions;

namespace GestionCommerciale.Application.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    public async Task<List<ClientDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var clients = await clientRepository.GetAllAsync(cancellationToken);
        return clients.Select(client => client.ToDto()).ToList();
    }

    public async Task<ClientDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = await GetExistingClientAsync(id, cancellationToken);
        return client.ToDto();
    }

    public async Task<ClientDto> CreateAsync(SaveClientDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureEmailIsUniqueAsync(dto.Email, excludedClientId: null, cancellationToken);

        var client = new Client();
        client.ApplyFrom(dto);

        clientRepository.Add(client);
        await clientRepository.SaveChangesAsync(cancellationToken);

        return client.ToDto(); // l'Id a été rempli par la base au SaveChanges
    }

    public async Task<ClientDto> UpdateAsync(int id, SaveClientDto dto, CancellationToken cancellationToken = default)
    {
        var client = await GetExistingClientAsync(id, cancellationToken);
        await EnsureEmailIsUniqueAsync(dto.Email, excludedClientId: id, cancellationToken);

        client.ApplyFrom(dto);
        await clientRepository.SaveChangesAsync(cancellationToken); // EF détecte les changements → UPDATE

        return client.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = await GetExistingClientAsync(id, cancellationToken);

        if (await clientRepository.HasOrdersAsync(id, cancellationToken))
            throw new BusinessRuleException("Impossible de supprimer ce client : il possède des commandes.");

        clientRepository.Remove(client);
        await clientRepository.SaveChangesAsync(cancellationToken);
    }


    private async Task<Client> GetExistingClientAsync(int id, CancellationToken cancellationToken) =>
        await clientRepository.GetByIdAsync(id, cancellationToken)
        ?? throw new NotFoundException($"Le client {id} est introuvable.");

    private async Task EnsureEmailIsUniqueAsync(string email, int? excludedClientId, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (await clientRepository.EmailExistsAsync(normalizedEmail, excludedClientId, cancellationToken))
            throw new BusinessRuleException($"Un client avec l'email {normalizedEmail} existe déjà.");
    }
}