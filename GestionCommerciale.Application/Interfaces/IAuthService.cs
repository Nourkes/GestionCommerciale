using GestionCommerciale.Application.DTOs.Auth;

namespace GestionCommerciale.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}