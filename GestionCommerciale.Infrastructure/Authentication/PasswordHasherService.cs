using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GestionCommerciale.Infrastructure.Authentication;

// Utilise le PasswordHasher d'ASP.NET Core Identity (PBKDF2 + sel aléatoire)
public class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) =>
        _hasher.HashPassword(null!, password);

    public bool Verify(string hashedPassword, string providedPassword) =>
        _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword)
            != PasswordVerificationResult.Failed;
}