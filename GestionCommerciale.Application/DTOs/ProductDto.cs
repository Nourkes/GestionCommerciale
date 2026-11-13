namespace GestionCommerciale.Application.DTOs.Products;

public record ProductDto(
    int Id,
    string Reference,
    string Name,
    string? Description,
    decimal UnitPriceHT,
    int StockQuantity,
    DateTime CreatedAt);