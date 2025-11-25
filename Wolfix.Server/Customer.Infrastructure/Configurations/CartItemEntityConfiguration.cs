using Customer.Domain.CustomerAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.ValueGenerators;

namespace Customer.Infrastructure.Configurations;

internal sealed class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");
        
        ConfigureBasicProperties(builder);
        
        ConfigureCustomerRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<CartItem> builder)
    {
        builder.Property(p => p.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(ci => ci.PhotoUrl)
            .IsRequired();

        builder.Property(ci => ci.Title)
            .IsRequired();

        builder.Property(ci => ci.PriceWithDiscount)
            .IsRequired();

        builder.Property(ci => ci.ProductId)
            .IsRequired();
    }

    private void ConfigureCustomerRelation(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasOne<Customer.Domain.CustomerAggregate.Customer>(ci => ci.Customer)
            .WithMany("_cartItems")
            .HasForeignKey(ci => ci.CustomerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation(ci => ci.Customer)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}