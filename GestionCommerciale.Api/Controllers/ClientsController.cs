using GestionCommerciale.Application.DTOs.Clients;
using GestionCommerciale.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionCommerciale.Api.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize] // toutes les routes exigent un token
public class ClientsController(IClientService clientService) : ControllerBase
{
    /// <summary>Liste de tous les clients.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ClientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ClientDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await clientService.GetAllAsync(cancellationToken));

    /// <summary>Détail d'un client.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await clientService.GetByIdAsync(id, cancellationToken));

    /// <summary>Crée un client.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClientDto>> Create(SaveClientDto dto, CancellationToken cancellationToken)
    {
        var client = await clientService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    /// <summary>Modifie un client.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClientDto>> Update(int id, SaveClientDto dto, CancellationToken cancellationToken)
        => Ok(await clientService.UpdateAsync(id, dto, cancellationToken));

    /// <summary>Supprime un client (refusé s'il a des commandes).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await clientService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}