using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Support.Domain.Entities;
using Support.Domain.Enums;

namespace Support.Infrastructure.Configurations;

public sealed class SupportRequestEntityConfiguration : IEntityTypeConfiguration<SupportRequest>
{
    public void Configure(EntityTypeBuilder<SupportRequest> builder)
    {
        builder.ToTable("SupportRequests");
        
        ConfigureBasicProperties(builder);
        
        ConfigureSupportRelation(builder);
    }
    
    private void ConfigureBasicProperties(EntityTypeBuilder<SupportRequest> builder)
    {
        builder.OwnsOne(sr => sr.FullName, fullName =>
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

        builder.OwnsOne(sr => sr.PhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(pn => pn.Value)
                .HasColumnName("PhoneNumber")
                .IsRequired();
        });

        builder.OwnsOne(sr => sr.BirthDate, birthDate =>
        {
            birthDate.Property(bd => bd.Value)
                .HasColumnName("BirthDate")
                .IsRequired();
        });

        builder.Property(sr => sr.CustomerId)
            .IsRequired();

        builder.Property(sr => sr.Title)
            .IsRequired();

        builder.Property(sr => sr.ProductId)
            .IsRequired(false)
            .HasDefaultValue(null);

        builder.Property(sr => sr.RequestContent)
            .IsRequired();

        builder.Property(sr => sr.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasDefaultValue(SupportRequestStatus.Pending);
        
        builder.Property(sr => sr.ResponseContent)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(sr => sr.CreatedAt)
            .IsRequired();

        builder.Property(sr => sr.ProcessedAt)
            .IsRequired(false)
            .HasDefaultValue(null);
    }

    private void ConfigureSupportRelation(EntityTypeBuilder<SupportRequest> builder)
    {
        builder.HasOne<Domain.Entities.Support>(sr => sr.ProcessedBy)
            .WithMany(sup => sup.SupportRequests)
            .HasForeignKey(sr => sr.SupportId)
            .IsRequired(false);
        builder.Navigation(sr => sr.ProcessedBy)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}