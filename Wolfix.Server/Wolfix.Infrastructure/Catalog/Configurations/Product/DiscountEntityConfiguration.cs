using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Product;

internal sealed class DiscountEntityConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts");
        
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Percent).IsRequired();
        builder.Property(d => d.ExpirationDateTime).IsRequired();
        builder.Property(d => d.Status).IsRequired().HasConversion<string>();
        
        builder.Property(d => d.ProductId).IsRequired();
        builder.HasIndex(d => d.ProductId);
    }
}