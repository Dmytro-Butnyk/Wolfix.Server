using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.ValueGenerators;

namespace Seller.Infrastructure.Configurations.SellerApplication;

internal sealed class SellerApplicationEntityConfiguration : IEntityTypeConfiguration<Domain.SellerApplicationAggregate.SellerApplication>
{
    public void Configure(EntityTypeBuilder<Domain.SellerApplicationAggregate.SellerApplication> builder)
    {
        builder.ToTable("SellerApplications");
        
        ConfigureBasicProperties(builder);
    }
    
    private void ConfigureBasicProperties(EntityTypeBuilder<Domain.SellerApplicationAggregate.SellerApplication> builder)
    {
        builder.Property(sa => sa.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();

        builder.Property(sa => sa.AccountId)
            .IsRequired();

        builder.Property(sa => sa.CategoryName)
            .IsRequired();

        builder.Property(sa => sa.BlobResourceId)
            .IsRequired();

        builder.Property(sa => sa.DocumentUrl)
            .IsRequired();
        
        builder.Property(sa => sa.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.OwnsOne(sa => sa.SellerProfileData, sellerProfileData =>
        {
            sellerProfileData.OwnsOne(spd => spd.FullName, fullName =>
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

            sellerProfileData.OwnsOne(spd => spd.PhoneNumber, phoneNumber =>
            {
                phoneNumber.Property(pn => pn.Value)
                    .HasColumnName("PhoneNumber")
                    .IsRequired();
            });

            sellerProfileData.OwnsOne(spd => spd.Address, address =>
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

            sellerProfileData.OwnsOne(spd => spd.BirthDate, birthDate =>
            {
                birthDate.Property(bd => bd.Value)
                    .HasColumnName("BirthDate")
                    .IsRequired();
            });
        });
    }
}