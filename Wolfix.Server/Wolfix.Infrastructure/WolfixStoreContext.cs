using Microsoft.EntityFrameworkCore;
using Wolfix.Domain.Catalog.CategoryAggregate;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.ProductAggregate;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;
using Wolfix.Domain.Shared;
using Wolfix.Infrastructure.Catalog.Configurations.Category;
using Wolfix.Infrastructure.Catalog.Configurations.Product;

namespace Wolfix.Infrastructure;

public sealed class WolfixStoreContext : DbContext
{
    public WolfixStoreContext() {}
    
    public WolfixStoreContext(DbContextOptions<WolfixStoreContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<BaseEntity>();
        
        #region Catalog
        #region Category
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductVariantEntityConfiguration());
        #endregion
        
        #region Product
        modelBuilder.ApplyConfiguration(new DiscountEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeValueEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductVariantValueEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewEntityConfiguration());
        #endregion
        #endregion
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (optionsBuilder.IsConfigured) return;
    //     
    //     var connectionString = "connection_string";
    //     optionsBuilder.UseNpgsql(connectionString);
    // }
    
    #region CATALOG
    internal DbSet<Product> Products { get; set; } // Aggregate 
    internal DbSet<ProductAttribute> ProductAttributes  { get; set; }
    internal DbSet<ProductVariant> ProductVariants  { get; set; }
    
    internal DbSet<Category> Categories { get; set; } // Aggregate
    internal DbSet<Discount>  Discounts { get; set; }
    internal DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
    internal DbSet<ProductVariantValue> ProductVariantValues { get; set; }
    #endregion CATALOG
}