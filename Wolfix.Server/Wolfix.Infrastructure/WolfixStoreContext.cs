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
    
    internal DbSet<Product> Products { get; set; } // Aggregate 
    internal DbSet<ProductAttribute> ProductAttributes  { get; set; }
    internal DbSet<ProductVariant> ProductVariants  { get; set; }
    
    internal DbSet<Category> Categories { get; set; } // Aggregate
    internal DbSet<Discount>  Discounts { get; set; }
    internal DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
    internal DbSet<ProductVariantValue> ProductVariantValues { get; set; }
    
    #endregion CATALOG
    
    
}