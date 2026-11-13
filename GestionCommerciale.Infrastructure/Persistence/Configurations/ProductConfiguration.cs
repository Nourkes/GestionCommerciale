using GestionCommerciale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionCommerciale.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Reference).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.UnitPriceHT).HasPrecision(18, 2);

        builder.HasIndex(p => p.Reference).IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Products_UnitPriceHT", "[UnitPriceHT] >= 0");
            t.HasCheckConstraint("CK_Products_StockQuantity", "[StockQuantity] >= 0");
        });
    }
}