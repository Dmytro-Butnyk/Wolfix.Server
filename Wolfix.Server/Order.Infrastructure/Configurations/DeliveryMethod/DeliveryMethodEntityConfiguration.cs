using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.DeliveryAggregate.Entities;
using Shared.Infrastructure.ValueGenerators;
using DeliveryMethodAggregate = Order.Domain.DeliveryAggregate.DeliveryMethod;

namespace Order.Infrastructure.Configurations.DeliveryMethod;

internal sealed class DeliveryMethodEntityConfiguration : IEntityTypeConfiguration<DeliveryMethodAggregate>
{
    public void Configure(EntityTypeBuilder<DeliveryMethodAggregate> builder)
    {
        ConfigureBasicProperties(builder);
        
        ConfigureCityRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<DeliveryMethodAggregate> builder)
    {
        builder.Property(dm => dm.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(dm => dm.Name)
            .IsRequired();

        builder.Ignore(dm => dm.Cities);
    }

    private void ConfigureCityRelation(EntityTypeBuilder<DeliveryMethodAggregate> builder)
    {
        builder.HasMany<City>("_cities")
            .WithOne(city => city.DeliveryMethod)
            .HasForeignKey(city => city.DeliveryMethodId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation("_cities")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}