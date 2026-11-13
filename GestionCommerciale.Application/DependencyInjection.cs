using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GestionCommerciale.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}