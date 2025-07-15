using Microsoft.EntityFrameworkCore;
using Wolfix.Domain.Catalog.CategoryAggregate;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.ProductAggregate;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;

namespace Wolfix.Infrastructure;

public sealed class WolfixStoreContext : DbContext
{
    public WolfixStoreContext() {}
    
    public WolfixStoreContext(DbContextOptions<WolfixStoreContext> options)
        : base(options) { }
    
    #region CATALOG
    
    public DbSet<Product> Products { get; set; } // Aggregate 
    public DbSet<ProductAttribute> ProductAttributes  { get; set; }
    public DbSet<ProductVariant> ProductVariants  { get; set; }
    
    public DbSet<Category> Categories { get; set; } // Aggregate
    public DbSet<Discount>  Discounts { get; set; }
    public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
    public DbSet<ProductVariantValue> ProductVariantValues { get; set; }
    
    #endregion CATALOG
    
    
}