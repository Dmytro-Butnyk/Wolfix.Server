using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;

namespace Wolfix.Infrastructure.TablesConfigurations.CatalogContext.ProductAggregate;

public sealed class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Percent)
            .IsRequired();

        builder.Property(d => d.ExpirationDateTime)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<string>() // храним enum как строку для читаемости
            .IsRequired();
    }
}