using GestionCommerciale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionCommerciale.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.FirstNameOrCompany).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.Address).HasMaxLength(250);

        builder.HasIndex(c => c.Email).IsUnique();
    }
}