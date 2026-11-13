using GestionCommerciale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionCommerciale.Infrastructure.Persistence.Configurations;

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.Property(l => l.UnitPrice).HasPrecision(18, 2);
        builder.Property(l => l.LineTotal).HasPrecision(18, 2);

        // On interdit de supprimer un produit présent dans une commande
        builder.HasOne(l => l.Product)
               .WithMany()
               .HasForeignKey(l => l.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t => t.HasCheckConstraint("CK_OrderLines_Quantity", "[Quantity] > 0"));
    }
}