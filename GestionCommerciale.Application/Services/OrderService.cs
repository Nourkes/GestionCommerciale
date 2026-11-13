using GestionCommerciale.Application.DTOs.Orders;
using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Application.Mappings;
using GestionCommerciale.Domain.Entities;
using GestionCommerciale.Domain.Enums;
using GestionCommerciale.Domain.Exceptions;

namespace GestionCommerciale.Application.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IProductRepository productRepository) : IOrderService
{

    public async Task<List<OrderSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await orderRepository.GetAllAsync(cancellationToken);
        return orders.Select(order => order.ToSummaryDto()).ToList();
    }

    public async Task<OrderDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await GetExistingOrderAsync(id, cancellationToken);
        return order.ToDetailDto();
    }


    public async Task<OrderDetailDto> CreateAsync(SaveOrderDto dto, CancellationToken cancellationToken = default)
    {
        var client = await GetExistingClientAsync(dto.ClientId, cancellationToken);
        var lines = await BuildLinesAsync(dto.Lines, cancellationToken);

        var order = new Order
        {
            OrderNumber = await GenerateOrderNumberAsync(cancellationToken),
            ClientId = client.Id,
            Client = client,
            OrderDate = dto.OrderDate ?? DateTime.UtcNow,
            Status = OrderStatus.Draft,
            Lines = lines
        };
        order.RecalculateTotals();

        orderRepository.Add(order);
        await orderRepository.SaveChangesAsync(cancellationToken);

        return order.ToDetailDto();
    }

    public async Task<OrderDetailDto> UpdateAsync(int id, SaveOrderDto dto, CancellationToken cancellationToken = default)
    {
        var order = await GetExistingOrderAsync(id, cancellationToken);
        EnsureIsDraft(order, "modifiée");

        var client = await GetExistingClientAsync(dto.ClientId, cancellationToken);
        var newLines = await BuildLinesAsync(dto.Lines, cancellationToken);

        order.ClientId = client.Id;
        order.Client = client;
        order.OrderDate = dto.OrderDate ?? order.OrderDate;

        // On remplace toutes les lignes : EF Core supprime les anciennes (orphelines) et insère les nouvelles
        order.Lines.Clear();
        order.Lines.AddRange(newLines);
        order.RecalculateTotals();

        await orderRepository.SaveChangesAsync(cancellationToken);
        return order.ToDetailDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await GetExistingOrderAsync(id, cancellationToken);

        if (order.Status == OrderStatus.Validated)
            throw new BusinessRuleException("Une commande validée ne peut pas être supprimée. Annulez-la plutôt.");

        orderRepository.Remove(order); 
        await orderRepository.SaveChangesAsync(cancellationToken);
    }


    public async Task<OrderDetailDto> ValidateAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await GetExistingOrderAsync(id, cancellationToken);
        EnsureIsDraft(order, "validée");

        // On revérifie le stock : il a pu baisser depuis la création du brouillon
        foreach (var line in order.Lines)
            EnsureStockIsSufficient(line.Product, line.Quantity);

        foreach (var line in order.Lines)
            line.Product.StockQuantity -= line.Quantity;

        order.Status = OrderStatus.Validated;

        // Un seul SaveChanges = une seule transaction : stock et statut sont enregistrés ensemble, ou pas du tout
        await orderRepository.SaveChangesAsync(cancellationToken);
        return order.ToDetailDto();
    }

    public async Task<OrderDetailDto> CancelAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await GetExistingOrderAsync(id, cancellationToken);

        if (order.Status == OrderStatus.Cancelled)
            throw new BusinessRuleException("Cette commande est déjà annulée.");

        // Si la commande était validée, on remet les produits en stock
        if (order.Status == OrderStatus.Validated)
        {
            foreach (var line in order.Lines)
                line.Product.StockQuantity += line.Quantity;
        }

        order.Status = OrderStatus.Cancelled;
        await orderRepository.SaveChangesAsync(cancellationToken);
        return order.ToDetailDto();
    }


    // Transforme les lignes reçues en lignes de commande, avec prix et totaux calculés côté serveur
    private async Task<List<OrderLine>> BuildLinesAsync(List<SaveOrderLineDto> lineDtos, CancellationToken cancellationToken)
    {
        // Si le même produit apparaît deux fois, on additionne les quantités
        var quantitiesByProduct = lineDtos
            .GroupBy(line => line.ProductId)
            .ToDictionary(group => group.Key, group => group.Sum(line => line.Quantity));

        // Tous les produits en une seule requête SQL
        var products = await productRepository.GetByIdsAsync(quantitiesByProduct.Keys, cancellationToken);

        var missingIds = quantitiesByProduct.Keys.Except(products.Select(p => p.Id)).ToList();
        if (missingIds.Count > 0)
            throw new NotFoundException($"Produit(s) introuvable(s) : {string.Join(", ", missingIds)}.");

        return products.Select(product =>
        {
            var quantity = quantitiesByProduct[product.Id];
            EnsureStockIsSufficient(product, quantity);

            return new OrderLine
            {
                ProductId = product.Id,
                Product = product,
                Quantity = quantity,
                UnitPrice = product.UnitPriceHT,              // prix figé au moment de la commande
                LineTotal = quantity * product.UnitPriceHT    // Quantité × Prix unitaire
            };
        }).ToList();
    }

    private async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken)
    {
        var prefix = $"CMD-{DateTime.UtcNow:yyyyMMdd}-";
        var lastNumber = await orderRepository.GetLastOrderNumberWithPrefixAsync(prefix, cancellationToken);

        var nextSequence = lastNumber is null ? 1 : int.Parse(lastNumber[prefix.Length..]) + 1;
        return $"{prefix}{nextSequence:D4}";
    }

    private static void EnsureStockIsSufficient(Product product, int requestedQuantity)
    {
        if (requestedQuantity > product.StockQuantity)
            throw new BusinessRuleException(
                $"Stock insuffisant pour {product.Reference} ({product.Name}) : " +
                $"demandé {requestedQuantity}, disponible {product.StockQuantity}.");
    }

    private static void EnsureIsDraft(Order order, string action)
    {
        if (order.Status != OrderStatus.Draft)
            throw new BusinessRuleException(
                $"La commande {order.OrderNumber} ne peut pas être {action} : son statut est {order.Status}.");
    }

    private async Task<Order> GetExistingOrderAsync(int id, CancellationToken cancellationToken) =>
        await orderRepository.GetByIdWithDetailsAsync(id, cancellationToken)
        ?? throw new NotFoundException($"La commande {id} est introuvable.");

    private async Task<Client> GetExistingClientAsync(int clientId, CancellationToken cancellationToken) =>
        await clientRepository.GetByIdAsync(clientId, cancellationToken)
        ?? throw new NotFoundException($"Le client {clientId} est introuvable.");
}