using GestionCommerciale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionCommerciale.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.HasIndex(u => u.Username).IsUnique();
    }
}