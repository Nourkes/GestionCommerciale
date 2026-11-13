namespace GestionCommerciale.Domain.Exceptions;

// Levée quand une règle métier n'est pas respectée, ex. stock insuffisant (→ HTTP 409)
public class BusinessRuleException(string message) : Exception(message);