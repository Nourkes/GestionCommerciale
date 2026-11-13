namespace GestionCommerciale.Application.DTOs.Orders;

public record OrderLineDto(
    int Id,
    int ProductId,
    string ProductReference,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);