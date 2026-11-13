using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Application.DTOs.Orders;

public class SaveOrderLineDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Le produit est obligatoire.")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être supérieure à zéro.")]
    public int Quantity { get; set; }
}