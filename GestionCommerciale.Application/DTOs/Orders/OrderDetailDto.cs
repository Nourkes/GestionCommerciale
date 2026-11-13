namespace GestionCommerciale.Application.DTOs.Orders;

public record OrderDetailDto(
    int Id,
    string OrderNumber,
    int ClientId,
    string ClientName,
    DateTime OrderDate,
    string Status,
    decimal TotalHT,
    decimal TotalVAT,
    decimal TotalTTC,
    List<OrderLineDto> Lines);