namespace Catalog.Application.Dto.Product.FullDto;

public sealed class ProductCategoryDto
{
    public Guid CategoryId { get; init; }    
    public string CategoryName { get; init; }
    public int Order { get; init; }   
}