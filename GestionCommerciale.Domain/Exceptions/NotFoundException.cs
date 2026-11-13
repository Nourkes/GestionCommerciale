namespace GestionCommerciale.Domain.Exceptions;

// Levée quand une ressource demandée n'existe pas (→ HTTP 404)
public class NotFoundException(string message) : Exception(message);