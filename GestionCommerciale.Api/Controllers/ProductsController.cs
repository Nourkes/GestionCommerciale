using GestionCommerciale.Application.DTOs.Products;
using GestionCommerciale.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionCommerciale.Api.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController(IProductService productService) : ControllerBase
{
    /// <summary>Liste de tous les produits.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await productService.GetAllAsync(cancellationToken));

    /// <summary>Détail d'un produit.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await productService.GetByIdAsync(id, cancellationToken));

    /// <summary>Crée un produit.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Create(SaveProductDto dto, CancellationToken cancellationToken)
    {
        var product = await productService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>Modifie un produit.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Update(int id, SaveProductDto dto, CancellationToken cancellationToken)
        => Ok(await productService.UpdateAsync(id, dto, cancellationToken));

    /// <summary>Supprime un produit (refusé s'il est utilisé dans une commande).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}