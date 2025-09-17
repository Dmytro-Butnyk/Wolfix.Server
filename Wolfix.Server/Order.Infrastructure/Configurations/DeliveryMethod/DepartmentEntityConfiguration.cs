using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.DeliveryAggregate.Entities;
using Shared.Infrastructure.ValueGenerators;

namespace Order.Infrastructure.Configurations.DeliveryMethod;

internal sealed class DepartmentEntityConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        ConfigureBasicProperties(builder);
        
        ConfigureCityRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<Department> builder)
    {
        builder.Property(pm => pm.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(d => d.Number)
            .IsRequired();

        builder.Property(d => d.Street)
            .IsRequired();

        builder.Property(d => d.HouseNumber)
            .IsRequired();
    }

    private void ConfigureCityRelation(EntityTypeBuilder<Department> builder)
    {
        builder.HasOne<City>(d => d.City)
            .WithMany("_departments")
            .HasForeignKey(d => d.CityId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation(d => d.City)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}