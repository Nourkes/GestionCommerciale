using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GestionCommerciale.Infrastructure.Persistence;

public class DbSeeder(AppDbContext db, IPasswordHasher passwordHasher, IConfiguration configuration)
{
    public async Task SeedAsync()
    {
        if (await db.Users.AnyAsync())
            return; 

        var username = configuration["DefaultAdmin:Username"] ?? "admin";
        var password = configuration["DefaultAdmin:Password"]
            ?? throw new InvalidOperationException("DefaultAdmin:Password est manquant dans appsettings.json");

        db.Users.Add(new User { Username = username, PasswordHash = passwordHasher.Hash(password) });
        await db.SaveChangesAsync();
    }
}