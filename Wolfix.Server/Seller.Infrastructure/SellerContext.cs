using Microsoft.EntityFrameworkCore;
using Seller.Domain.SellerAggregate.Entities;
using Seller.Infrastructure.Configurations;
using Shared.Infrastructure;

namespace Seller.Infrastructure;

public sealed class SellerContext : DbContext, IContextWithConfigurations
{
    public SellerContext() { }
    
    public SellerContext(DbContextOptions<SellerContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("seller");
        ApplyConfigurations(modelBuilder);
        
    }

    public void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SellerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SellerCategoryEntityConfiguration());
    }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (optionsBuilder.IsConfigured) return;
    //     
    //     
    //     optionsBuilder.UseNpgsql(connectionString);
    // }
    
    internal DbSet<Seller.Domain.SellerAggregate.Seller> Sellers { get; set; } //Aggregate
    internal DbSet<SellerCategory> SellerCategories { get; set; }
}