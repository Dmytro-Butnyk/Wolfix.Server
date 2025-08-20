using Customer.Domain.CustomerAggregate.Entities;
using Customer.Domain.CustomerAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Infrastructure.Configurations;

internal sealed class CustomerEntityConfiguration : IEntityTypeConfiguration<Domain.CustomerAggregate.Customer>
{
    public void Configure(EntityTypeBuilder<Domain.CustomerAggregate.Customer> builder)
    {
        builder.ToTable("Customers");
        
        ConfigureBasicProperties(builder);
        
        ConfigureFavoriteItemsRelation(builder);
        
        ConfigureCartItemsRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<Domain.CustomerAggregate.Customer> builder)
    {
        const string nullMarker = "_____NULL_____";
        
        builder.Property(c => c.PhotoUrl)
            .IsRequired(false);
        
        builder.Property(c => c.FullName)
            .IsRequired(false)
            .HasConversion<string>(
                fullName => fullName == null ? nullMarker : fullName.ToString(),
                fullNameString => fullNameString == nullMarker ? null : (FullName)fullNameString
            );
        
        builder.Property(c => c.PhoneNumber)
            .IsRequired(false)
            .HasConversion<string>(
                phoneNumber => phoneNumber == null ? nullMarker : phoneNumber.Value,
                phoneNumberString => phoneNumberString == nullMarker ? null : (PhoneNumber)phoneNumberString
            );

        builder.Property(c => c.Address)
            .IsRequired(false)
            .HasConversion<string>(
                address => address == null ? nullMarker : address.ToString(),
                addressString => addressString == nullMarker ? null : (Address)addressString
            );

        builder.Property(c => c.BirthDate)
            .IsRequired(false)
            .HasConversion<string>(
                birthDate => birthDate == null ? nullMarker : birthDate.ToString(),
                birthDateString => birthDateString == nullMarker ? null : (BirthDate)birthDateString
            );

        builder.Property(c => c.BonusesAmount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.AccountId)
            .IsRequired();

        builder.Ignore(c => c.FavoriteItems);

        builder.Ignore(c => c.CartItems);
    }
    
    private void ConfigureFavoriteItemsRelation(EntityTypeBuilder<Domain.CustomerAggregate.Customer> builder)
    {
        builder.HasMany<FavoriteItem>("_favoriteItems")
            .WithOne(fi => fi.Customer)
            .HasForeignKey(fi => fi.CustomerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_favoriteItems")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private void ConfigureCartItemsRelation(EntityTypeBuilder<Domain.CustomerAggregate.Customer> builder)
    {
        builder.HasMany<CartItem>("_cartItems")
            .WithOne(ci => ci.Customer)
            .HasForeignKey(ci => ci.CustomerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_cartItems")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}