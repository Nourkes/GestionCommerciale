using GestionCommerciale.Application.DTOs.Orders;
using GestionCommerciale.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionCommerciale.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>Liste des commandes (la plus récente en premier).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderSummaryDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await orderService.GetAllAsync(cancellationToken));

    /// <summary>Détail d'une commande avec ses lignes.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDetailDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await orderService.GetByIdAsync(id, cancellationToken));

    /// <summary>Crée une commande au statut Brouillon.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDetailDto>> Create(SaveOrderDto dto, CancellationToken cancellationToken)
    {
        var order = await orderService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>Modifie une commande (uniquement au statut Brouillon).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDetailDto>> Update(int id, SaveOrderDto dto, CancellationToken cancellationToken)
        => Ok(await orderService.UpdateAsync(id, dto, cancellationToken));

    /// <summary>Supprime une commande (refusé si elle est validée).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await orderService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Valide une commande et met à jour le stock des produits.</summary>
    [HttpPost("{id:int}/validate")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDetailDto>> Validate(int id, CancellationToken cancellationToken)
        => Ok(await orderService.ValidateAsync(id, cancellationToken));

    /// <summary>Annule une commande (remise en stock si elle était validée).</summary>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDetailDto>> Cancel(int id, CancellationToken cancellationToken)
        => Ok(await orderService.CancelAsync(id, cancellationToken));
}