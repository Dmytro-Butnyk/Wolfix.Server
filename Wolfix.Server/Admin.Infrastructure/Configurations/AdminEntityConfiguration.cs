using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.ValueGenerators;

namespace Admin.Infrastructure.Configurations;

internal sealed class AdminEntityConfiguration : IEntityTypeConfiguration<Domain.AdminAggregate.Admin>
{
    public void Configure(EntityTypeBuilder<Domain.AdminAggregate.Admin> builder)
    {
        builder.ToTable("Admins");
        
        builder.Property(admin => admin.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();

        builder.Property(admin => admin.AccountId)
            .IsRequired();

        builder.OwnsOne(admin => admin.FullName, fullName =>
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

        builder.OwnsOne(admin => admin.PhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(pn => pn.Value)
                .HasColumnName("PhoneNumber")
                .IsRequired();
        });

        builder.Property(admin => admin.Type)
            .IsRequired()
            .HasConversion<string>();
    }
}