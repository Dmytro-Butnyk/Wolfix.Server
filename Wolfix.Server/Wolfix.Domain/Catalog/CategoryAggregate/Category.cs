using System.Net;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.CategoryAggregate;

public sealed class Category : BaseEntity
{
    public Category? Parent { get; private set; }
    
    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    private readonly List<Guid> _productIds = [];
    public IReadOnlyCollection<Guid> ProductIds => _productIds.AsReadOnly();
    
    private readonly List<ProductVariant> _productVariants = [];
    public IReadOnlyCollection<ProductVariant> ProductVariants => _productVariants.AsReadOnly();
    
    private Category() { }

    private Category(string name, string? description = null, Category? parent = null)
    {
        Name = name;
        Description = description;
        Parent = parent;
    }

    public static Result<Category> Create(string name, string? description = null, Category? parent = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Category>.Failure($"{nameof(name)} is required");
        }
        
        if (description != null && string.IsNullOrWhiteSpace(description))
        {
            return Result<Category>.Failure($"{nameof(description)} is required");
        }

        var category = new Category(name, description, parent);
        return Result<Category>.Success(category, HttpStatusCode.Created);
    }
}