namespace Catalog.Domain.ValueObjects.FullProductDto;

public sealed class ProductCategoriesValueObject
{
    public Guid CategoryId { get; init; }    
    public string CategoryName { get; init; }
    public int Order { get; init; }
}