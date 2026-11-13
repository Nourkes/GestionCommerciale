using GestionCommerciale.Domain.Entities;

namespace GestionCommerciale.Application.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}