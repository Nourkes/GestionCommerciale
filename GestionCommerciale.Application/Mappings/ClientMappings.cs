using GestionCommerciale.Application.DTOs.Clients;
using GestionCommerciale.Domain.Entities;

namespace GestionCommerciale.Application.Mappings;

// Conversions entre l'entité Client et ses DTOs, écrites une seule fois
public static class ClientMappings
{
    public static ClientDto ToDto(this Client client) => new(
        client.Id,
        client.LastName,
        client.FirstNameOrCompany,
        client.Email,
        client.Phone,
        client.Address,
        client.CreatedAt);

    // Copie les valeurs du DTO dans l'entité (création et modification)
    public static void ApplyFrom(this Client client, SaveClientDto dto)
    {
        client.LastName = dto.LastName.Trim();
        client.FirstNameOrCompany = dto.FirstNameOrCompany.Trim();
        client.Email = dto.Email.Trim().ToLowerInvariant();
        client.Phone = dto.Phone.Trim();
        client.Address = dto.Address.Trim();
    }
}