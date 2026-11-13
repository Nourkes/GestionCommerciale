namespace GestionCommerciale.Domain.Entities;

public class OrderLine
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }  // prix figé au moment de la commande
    public decimal LineTotal { get; set; }  // Quantity × UnitPrice
}