using GestionCommerciale.Application.DTOs.Orders;
using GestionCommerciale.Domain.Entities;

namespace GestionCommerciale.Application.Mappings;

public static class OrderMappings
{
    public static OrderSummaryDto ToSummaryDto(this Order order) => new(
        order.Id,
        order.OrderNumber,
        order.ClientId,
        order.Client.GetDisplayName(),
        order.OrderDate,
        order.Status.ToString(),
        order.Lines.Count,
        order.TotalHT,
        order.TotalTTC);

    public static OrderDetailDto ToDetailDto(this Order order) => new(
        order.Id,
        order.OrderNumber,
        order.ClientId,
        order.Client.GetDisplayName(),
        order.OrderDate,
        order.Status.ToString(),
        order.TotalHT,
        order.TotalTTC - order.TotalHT, // montant de la TVA
        order.TotalTTC,
        order.Lines.Select(line => line.ToDto()).ToList());

    public static OrderLineDto ToDto(this OrderLine line) => new(
        line.Id,
        line.ProductId,
        line.Product.Reference,
        line.Product.Name,
        line.Quantity,
        line.UnitPrice,
        line.LineTotal);

    private static string GetDisplayName(this Client client) =>
        $"{client.LastName} {client.FirstNameOrCompany}".Trim();
}