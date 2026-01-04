using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.ValueGenerators;
using Support.Domain.Entities;

namespace Support.Infrastructure.Configurations;

public sealed class SupportEntityConfiguration : IEntityTypeConfiguration<Domain.Entities.Support>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Support> builder)
    {
        builder.ToTable("Supports");
        
        ConfigureBasicProperties(builder);
    }
    
    private static void ConfigureBasicProperties(EntityTypeBuilder<Domain.Entities.Support> builder)
    {
        builder.Property(s => s.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();

        builder.OwnsOne(sup => sup.FullName, fullName =>
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

        builder.Property(sup => sup.AccountId)
            .IsRequired();
    }
}