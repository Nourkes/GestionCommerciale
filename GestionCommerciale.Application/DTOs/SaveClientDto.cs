using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Application.DTOs.Clients;

public class SaveClientDto
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [MaxLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom ou la raison sociale est obligatoire.")]
    [MaxLength(150)]
    public string FirstNameOrCompany { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est obligatoire.")]
    [EmailAddress(ErrorMessage = "Le format de l'email est invalide.")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Le format du téléphone est invalide.")]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;
}