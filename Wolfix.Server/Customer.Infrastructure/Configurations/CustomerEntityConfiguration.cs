using Customer.Domain.CustomerAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.ValueGenerators;

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
        
        builder.Property(c => c.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(c => c.PhotoUrl)
            .IsRequired(false);
        
        builder.OwnsOne(c => c.FullName, fullName =>
        {
            fullName.Property(fn => fn.FirstName)
                .HasColumnName("FirstName")
                .IsRequired();

            fullName.Property(fn => fn.LastName)
                .HasColumnName("LastName")
                .IsRequired();

            fullName.Property(fn => fn.MiddleName)
                .HasColumnName("MiddleName")
                .IsRequired();
        });
        
        builder.Property(c => c.PhoneNumber)
            .IsRequired(false)
            .HasConversion<string>(
                phoneNumber => phoneNumber == null ? nullMarker : phoneNumber.Value,
                phoneNumberString => phoneNumberString == nullMarker ? null : (PhoneNumber)phoneNumberString
            );

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.City)
                .HasColumnName("City")
                .IsRequired();

            address.Property(a => a.Street)
                .HasColumnName("Street")
                .IsRequired();

            address.Property(a => a.HouseNumber)
                .HasColumnName("HouseNumber")
                .IsRequired();

            address.Property(a => a.ApartmentNumber)
                .HasColumnName("ApartmentNumber")
                .IsRequired(false);
        });

        builder.Property(c => c.BirthDate)
            .IsRequired(false)
            .HasConversion<string>(
                birthDate => birthDate == null ? nullMarker : birthDate.ToString(),
                birthDateString => birthDateString == nullMarker ? null : (BirthDate)birthDateString
            );

        builder.Property(c => c.BonusesAmount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.OwnsOne(c => c.ViolationStatus, vs =>
        {
            vs.Property(v => v.ViolationsCount)
                .HasColumnName("ViolationsCount")
                .IsRequired()
                .HasDefaultValue(0);

            vs.Property(v => v.Status)
                .HasColumnName("AccountStatus")
                .IsRequired()
                .HasConversion<string>();
        });

        builder.Property(c => c.AccountId)
            .IsRequired();

        builder.Ignore(c => c.FavoriteItems);

        builder.Ignore(c => c.CartItems);

        builder.Ignore(c => c.TotalCartPriceWithoutBonuses);
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