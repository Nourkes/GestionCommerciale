using GestionCommerciale.Application.DTOs.Products;
using GestionCommerciale.Domain.Entities;

namespace GestionCommerciale.Application.Mappings;

public static class ProductMappings
{
    public static ProductDto ToDto(this Product product) => new(
        product.Id,
        product.Reference,
        product.Name,
        product.Description,
        product.UnitPriceHT,
        product.StockQuantity,
        product.CreatedAt);

    public static void ApplyFrom(this Product product, SaveProductDto dto)
    {
        product.Reference = dto.Reference.Trim().ToUpperInvariant(); 
        product.Name = dto.Name.Trim();
        product.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        product.UnitPriceHT = dto.UnitPriceHT;
        product.StockQuantity = dto.StockQuantity;
    }
}