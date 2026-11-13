using GestionCommerciale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionCommerciale.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(30);
        builder.HasIndex(o => o.OrderNumber).IsUnique();

        builder.Property(o => o.TotalHT).HasPrecision(18, 2);
        builder.Property(o => o.TotalTTC).HasPrecision(18, 2);

        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

        // Un client a plusieurs commandes ; on interdit de supprimer un client qui a des commandes
        builder.HasOne(o => o.Client)
               .WithMany(c => c.Orders)
               .HasForeignKey(o => o.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

        // Composition : supprimer la commande supprime ses lignes
        builder.HasMany(o => o.Lines)
               .WithOne(l => l.Order)
               .HasForeignKey(l => l.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}