namespace GestionCommerciale.Application.DTOs.Orders;

public record OrderSummaryDto(
    int Id,
    string OrderNumber,
    int ClientId,
    string ClientName,
    DateTime OrderDate,
    string Status,
    int LineCount,
    decimal TotalHT,
    decimal TotalTTC);