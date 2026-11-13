using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Infrastructure.Authentication;
using GestionCommerciale.Infrastructure.Persistence;
using GestionCommerciale.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestionCommerciale.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Base de données
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Réglages JWT lus depuis appsettings.json
        var jwtSettings = new JwtSettings
        {
            Key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key est manquant"),
            Issuer = configuration["Jwt:Issuer"] ?? string.Empty,
            Audience = configuration["Jwt:Audience"] ?? string.Empty,
            ExpirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60")
        };
        services.AddSingleton(jwtSettings);

        // Implémentations des interfaces définies dans Application
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasherService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<DbSeeder>();

        return services;
    }
}