using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Application.DTOs.Orders;

public class SaveOrderDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Le client est obligatoire.")]
    public int ClientId { get; set; }

    public DateTime? OrderDate { get; set; }

    [MinLength(1, ErrorMessage = "La commande doit contenir au moins une ligne.")]
    public List<SaveOrderLineDto> Lines { get; set; } = new();
}