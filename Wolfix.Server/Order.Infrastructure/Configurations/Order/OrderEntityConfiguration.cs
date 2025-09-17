using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.OrderAggregate.Entities;
using Shared.Infrastructure.ValueGenerators;
using OrderAggregate = Order.Domain.OrderAggregate.Order;

namespace Order.Infrastructure.Configurations.Order;

internal sealed class OrderEntityConfiguration : IEntityTypeConfiguration<OrderAggregate>
{
    public void Configure(EntityTypeBuilder<OrderAggregate> builder)
    {
        ConfigureBasicProperties(builder);
        
        ConfigureOrderItemRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<OrderAggregate> builder)
    {
        builder.Property(order => order.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();

        builder.OwnsOne(order => order.CustomerInfo, customerInfo =>
        {
            customerInfo.OwnsOne(ci => ci.FullName, fullName =>
            {
                fullName.Property(fn => fn.FirstName)
                    .HasColumnName("Customer_FirstName")
                    .IsRequired();
                
                fullName.Property(fn => fn.LastName)
                    .HasColumnName("Customer_LastName")
                    .IsRequired();

                fullName.Property(fn => fn.MiddleName)
                    .HasColumnName("Customer_MiddleName")
                    .IsRequired();
            });

            customerInfo.OwnsOne(ci => ci.PhoneNumber, phoneNumber =>
            {
                phoneNumber.Property(pn => pn.Value)
                    .HasColumnName("Customer_PhoneNumber")
                    .IsRequired();
            });

            customerInfo.OwnsOne(ci => ci.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Customer_Email")
                    .IsRequired();
            });
        });

        builder.Property(order => order.CustomerId)
            .IsRequired();

        builder.OwnsOne(order => order.RecipientInfo, recipientInfo =>
        {
            recipientInfo.OwnsOne(ri => ri.FullName, fullName =>
            {
                fullName.Property(fn => fn.FirstName)
                    .HasColumnName("Recipient_FirstName")
                    .IsRequired();

                fullName.Property(fn => fn.LastName)
                    .HasColumnName("Recipient_LastName")
                    .IsRequired();

                fullName.Property(fn => fn.MiddleName)
                    .HasColumnName("Recipient_MiddleName")
                    .IsRequired();
            });

            recipientInfo.OwnsOne(ri => ri.PhoneNumber, phoneNumber =>
            {
                phoneNumber.Property(pn => pn.Value)
                    .HasColumnName("Recipient_PhoneNumber")
                    .IsRequired();
            });
        });

        builder.Property(order => order.PaymentOption)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(order => order.PaymentStatus)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsOne(order => order.DeliveryInfo, deliveryInfo =>
        {
            deliveryInfo.Property(di => di.Option)
                .HasColumnName("DeliveryOption")
                .HasConversion<string>()
                .IsRequired();
            
            deliveryInfo.Property(di => di.Number)
                .HasColumnName("DeliveryNumber")
                .IsRequired(false);
            
            deliveryInfo.Property(di => di.City)
                .HasColumnName("DeliveryCity")
                .IsRequired();

            deliveryInfo.Property(di => di.Street)
                .HasColumnName("DeliveryStreet")
                .IsRequired();

            deliveryInfo.Property(di => di.HouseNumber)
                .HasColumnName("DeliveryHouseNumber")
                .IsRequired();
        });
        
        builder.Property(order => order.DeliveryMethodName)
            .IsRequired();

        builder.Property(order => order.WithBonuses)
            .IsRequired();

        builder.Property(order => order.UsedBonusesAmount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(order => order.Price)
            .IsRequired();
        
        builder.Property(order => order.PaymentIntentId)
            .IsRequired(false);

        builder.Property(order => order.CreatedAt)
            .IsRequired();
        
        builder.Ignore(order => order.OrderItems);
    }

    private void ConfigureOrderItemRelation(EntityTypeBuilder<OrderAggregate> builder)
    {
        builder.HasMany<OrderItem>("_orderItems")
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation("_orderItems")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}