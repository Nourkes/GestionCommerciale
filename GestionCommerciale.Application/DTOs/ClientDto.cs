namespace GestionCommerciale.Application.DTOs.Clients;

public record ClientDto(
    int Id,
    string LastName,
    string FirstNameOrCompany,
    string Email,
    string Phone,
    string Address,
    DateTime CreatedAt);