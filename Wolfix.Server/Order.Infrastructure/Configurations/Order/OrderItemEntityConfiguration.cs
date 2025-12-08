using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.OrderAggregate.Entities;
using Shared.Infrastructure.ValueGenerators;

namespace Order.Infrastructure.Configurations.Order;

internal sealed class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        
        ConfigureBasicProperties(builder);
        
        ConfigureOrderRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(oi => oi.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(oi => oi.PhotoUrl)
            .IsRequired();
        
        builder.Property(oi => oi.Title)
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Property(oi => oi.Price)
            .IsRequired();

        builder.Property(oi => oi.CartItemId)
            .IsRequired();

        builder.Property(oi => oi.ProductId)
            .IsRequired();

        builder.Property(oi => oi.SellerId)
            .IsRequired();
    }

    private void ConfigureOrderRelation(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasOne<Domain.OrderAggregate.Order>(oi => oi.Order)
            .WithMany("_orderItems")
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation(oi => oi.Order)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}