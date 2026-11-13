using GestionCommerciale.Application.DTOs.Auth;
using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Domain.Exceptions;

namespace GestionCommerciale.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
            throw new UnauthorizedException("Nom d'utilisateur ou mot de passe incorrect");

        var (token, expiresAt) = tokenService.GenerateToken(user);
        return new LoginResponseDto(token, expiresAt, user.Username);
    }
}