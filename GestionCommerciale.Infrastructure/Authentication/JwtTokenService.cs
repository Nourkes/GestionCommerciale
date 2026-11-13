using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace GestionCommerciale.Infrastructure.Authentication;

public class JwtTokenService(JwtSettings settings) : ITokenService
{
    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Signature avec la clé secrète : personne ne peut modifier le token sans la connaître
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}