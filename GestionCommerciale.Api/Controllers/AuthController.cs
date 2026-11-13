using GestionCommerciale.Application.DTOs.Auth;
using GestionCommerciale.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionCommerciale.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request, CancellationToken cancellationToken)
        => Ok(await authService.LoginAsync(request, cancellationToken));
}