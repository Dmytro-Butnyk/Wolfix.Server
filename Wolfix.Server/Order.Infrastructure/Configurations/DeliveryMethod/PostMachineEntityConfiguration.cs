using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.DeliveryAggregate.Entities;
using Shared.Infrastructure.ValueGenerators;

namespace Order.Infrastructure.Configurations.DeliveryMethod;

internal sealed class PostMachineEntityConfiguration : IEntityTypeConfiguration<PostMachine>
{
    public void Configure(EntityTypeBuilder<PostMachine> builder)
    {
        builder.ToTable("PostMachines");
        
        ConfigureBasicProperties(builder);
        
        ConfigureCityRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<PostMachine> builder)
    {
        builder.Property(pm => pm.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();

        builder.Property(pm => pm.Number)
            .IsRequired();

        builder.Property(pm => pm.Street)
            .IsRequired();

        builder.Property(pm => pm.HouseNumber)
            .IsRequired();

        builder.Property(pm => pm.Note)
            .IsRequired(false);
    }

    private void ConfigureCityRelation(EntityTypeBuilder<PostMachine> builder)
    {
        builder.HasOne<City>(pm => pm.City)
            .WithMany("_postMachines")
            .HasForeignKey(pm => pm.CityId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation(pm => pm.City)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}