using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.DeliveryAggregate.Entities;
using Shared.Infrastructure.ValueGenerators;
using DeliveryMethodAggregate = Order.Domain.DeliveryAggregate.DeliveryMethod;

namespace Order.Infrastructure.Configurations.DeliveryMethod;

internal sealed class CityEntityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");
        
        ConfigureBasicProperties(builder);
        
        ConfigureDepartmentRelation(builder);
        
        ConfigurePostMachineRelation(builder);
        
        ConfigureDeliveryMethodRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<City> builder)
    {
        builder.Property(pm => pm.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(c => c.Name)
            .IsRequired();

        builder.Ignore(c => c.Departments);
        builder.Ignore(c => c.PostMachines);
    }

    private void ConfigureDepartmentRelation(EntityTypeBuilder<City> builder)
    {
        builder.HasMany<Department>("_departments")
            .WithOne(d => d.City)
            .HasForeignKey(d => d.CityId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation("_departments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private void ConfigurePostMachineRelation(EntityTypeBuilder<City> builder)
    {
        builder.HasMany<PostMachine>("_postMachines")
            .WithOne(pm => pm.City)
            .HasForeignKey(pm => pm.CityId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation("_postMachines")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private void ConfigureDeliveryMethodRelation(EntityTypeBuilder<City> builder)
    {
        builder.HasOne<DeliveryMethodAggregate>(city => city.DeliveryMethod)
            .WithMany("_cities")
            .HasForeignKey(city => city.DeliveryMethodId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation(city => city.DeliveryMethod)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}