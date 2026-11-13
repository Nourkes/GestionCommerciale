using GestionCommerciale.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GestionCommerciale.Api.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Associe chaque type d'exception à un code HTTP et un titre
        var (statusCode, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Ressource introuvable"),
            BusinessRuleException => (StatusCodes.Status409Conflict, "Règle métier non respectée"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Authentification échouée"),
            _ => (StatusCodes.Status500InternalServerError, "Erreur interne du serveur")
        };

        // Les erreurs imprévues sont enregistrées dans les logs pour le développeur
        if (statusCode == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erreur non gérée : {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            // On ne montre jamais les détails techniques d'une erreur 500 au client
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "Une erreur inattendue est survenue."
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // l'exception est gérée : ASP.NET Core n'a rien d'autre à faire
    }
}