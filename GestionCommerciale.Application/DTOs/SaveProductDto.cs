using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Application.DTOs.Products;

public class SaveProductDto
{
    [Required(ErrorMessage = "La référence est obligatoire.")]
    [MaxLength(50, ErrorMessage = "La référence ne doit pas dépasser 50 caractères.")]
    public string Reference { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom du produit est obligatoire.")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(0, 999999999, ErrorMessage = "Le prix doit être positif ou nul.")]
    public decimal UnitPriceHT { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Le stock doit être positif ou nul.")]
    public int StockQuantity { get; set; }
}