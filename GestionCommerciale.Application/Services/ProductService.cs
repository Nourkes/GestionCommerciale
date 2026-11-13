using GestionCommerciale.Application.DTOs.Products;
using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Application.Mappings;
using GestionCommerciale.Domain.Entities;
using GestionCommerciale.Domain.Exceptions;

namespace GestionCommerciale.Application.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        return products.Select(product => product.ToDto()).ToList();
    }

    public async Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await GetExistingProductAsync(id, cancellationToken);
        return product.ToDto();
    }

    public async Task<ProductDto> CreateAsync(SaveProductDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureReferenceIsUniqueAsync(dto.Reference, excludedProductId: null, cancellationToken);

        var product = new Product();
        product.ApplyFrom(dto);

        productRepository.Add(product);
        await productRepository.SaveChangesAsync(cancellationToken);

        return product.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(int id, SaveProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await GetExistingProductAsync(id, cancellationToken);
        await EnsureReferenceIsUniqueAsync(dto.Reference, excludedProductId: id, cancellationToken);

        product.ApplyFrom(dto);
        await productRepository.SaveChangesAsync(cancellationToken);

        return product.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await GetExistingProductAsync(id, cancellationToken);

        // Un produit déjà commandé doit rester en base pour garder l'historique des commandes
        if (await productRepository.IsUsedInOrdersAsync(id, cancellationToken))
            throw new BusinessRuleException("Impossible de supprimer ce produit : il est utilisé dans des commandes.");

        productRepository.Remove(product);
        await productRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Product> GetExistingProductAsync(int id, CancellationToken cancellationToken) =>
        await productRepository.GetByIdAsync(id, cancellationToken)
        ?? throw new NotFoundException($"Le produit {id} est introuvable.");

    private async Task EnsureReferenceIsUniqueAsync(string reference, int? excludedProductId, CancellationToken cancellationToken)
    {
        var normalizedReference = reference.Trim().ToUpperInvariant();
        if (await productRepository.ReferenceExistsAsync(normalizedReference, excludedProductId, cancellationToken))
            throw new BusinessRuleException($"Un produit avec la référence {normalizedReference} existe déjà.");
    }
}