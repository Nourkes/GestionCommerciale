using GestionCommerciale.Domain.Enums;

namespace GestionCommerciale.Domain.Entities;

public class Order
{
    public const decimal VatRate = 0.19m; // TVA fixe de 19 % 

    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public decimal TotalHT { get; private set; }
    public decimal TotalTTC { get; private set; }

    public List<OrderLine> Lines { get; set; } = new();

    // Total HT = somme des lignes ; Total TTC = HT + 19 %
    public void RecalculateTotals()
    {
        TotalHT = Lines.Sum(line => line.LineTotal);
        TotalTTC = Math.Round(TotalHT * (1 + VatRate), 2);
    }
}