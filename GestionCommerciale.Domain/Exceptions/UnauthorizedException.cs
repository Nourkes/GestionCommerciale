namespace GestionCommerciale.Domain.Exceptions;

// Levée quand les identifiants sont incorrects (→ HTTP 401)
public class UnauthorizedException(string message) : Exception(message);