using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.CategoryAggregate.Entities;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Entities;
using Catalog.Infrastructure.Configurations.Category;
using Catalog.Infrastructure.Configurations.Product;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;

namespace Catalog.Infrastructure;

internal sealed class CatalogContext : DbContext, IContextWithConfigurations
{
    public CatalogContext() {}
    
    public CatalogContext(DbContextOptions<CatalogContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("catalog");
        ApplyConfigurations(modelBuilder);
    }
    
    public void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        //Category
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductVariantEntityConfiguration());
        
        //Product
        modelBuilder.ApplyConfiguration(new DiscountEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeValueEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductVariantValueEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductMediaConfiguration());
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (optionsBuilder.IsConfigured) return;
    //     
    //     optionsBuilder.UseNpgsql();
    // }
    
    internal DbSet<Product> Products { get; set; } // Aggregate 
    internal DbSet<ProductAttribute> ProductAttributes  { get; set; }
    internal DbSet<ProductVariant> ProductVariants  { get; set; }
    internal DbSet<ProductMedia> ProductMedias { get; set; }
    
    internal DbSet<Category> Categories { get; set; } // Aggregate
    internal DbSet<Discount>  Discounts { get; set; }
    internal DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
    internal DbSet<ProductVariantValue> ProductVariantValues { get; set; }
}