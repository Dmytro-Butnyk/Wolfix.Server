using Customer.Domain.CustomerAggregate.Entities;
using Customer.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;

namespace Customer.Infrastructure;

public sealed class CustomerContext : DbContext, IContextWithConfigurations
{
    public CustomerContext() { }

    public CustomerContext(DbContextOptions<CustomerContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        ApplyConfigurations(modelBuilder);
        
        modelBuilder.HasDefaultSchema("customer");
    }

    public void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CartItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new FavoriteItemEntityConfiguration());
    }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (optionsBuilder.IsConfigured) return;
    //     
    //     var connectionString = "connection_string";
    //     optionsBuilder.UseNpgsql(connectionString);
    // }
    
    internal DbSet<Customer.Domain.CustomerAggregate.Customer> Customers { get; set; } //Aggregate
    internal DbSet<CartItem> CartItems { get; set; }
    internal DbSet<FavoriteItem> FavoriteItems { get; set; }
}