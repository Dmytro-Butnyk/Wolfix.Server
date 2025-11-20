using Microsoft.EntityFrameworkCore;
using Order.Domain.DeliveryAggregate;
using Order.Domain.DeliveryAggregate.Entities;
using Order.Domain.OrderAggregate.Entities;
using Order.Infrastructure.Configurations.DeliveryMethod;
using Order.Infrastructure.Configurations.Order;
using Shared.Infrastructure;

namespace Order.Infrastructure;

public sealed class OrderContext : DbContext, IContextWithConfigurations
{
    public OrderContext() { }
    
    public OrderContext(DbContextOptions<OrderContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("order");
        ApplyConfigurations(modelBuilder);
    }

    public void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
        
        modelBuilder.ApplyConfiguration(new CityEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DeliveryMethodEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PostMachineEntityConfiguration());
    }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (optionsBuilder.IsConfigured) return;
    //     
    //     
    //     optionsBuilder.UseNpgsql("");
    // }
    
    internal DbSet<Domain.OrderAggregate.Order> Orders { get; set; } // Aggregate 
    internal DbSet<OrderItem> OrderItems { get; set; }
    
    internal DbSet<DeliveryMethod> DeliveryMethods { get; set; }
    internal DbSet<City> Cities { get; set; }
    internal DbSet<Department> Departments { get; set; }
    internal DbSet<PostMachine> PostMachines { get; set; }
}