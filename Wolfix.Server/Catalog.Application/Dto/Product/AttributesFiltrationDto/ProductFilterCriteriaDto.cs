using Shared.Domain.Models;

namespace Catalog.Application.Dto.Product.AttributesFiltrationDto;

public sealed record ProductFilterCriteriaDto
{
    public Guid CategoryId { get; init; }
    public required IReadOnlyCollection<FiltrationAttributeDto> FiltrationAttribute { get; init; }
    
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    
    public int PageSize { get; init; } = 30;
    public int PageNumber { get; init; } = 1;
    public int SkipCount => PageSize * (PageNumber - 1);
    
    public VoidResult EnsureValid()
    {
        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
            return VoidResult.Failure("Min price cannot be greater than max price.");
            
        if (MinPrice < 0 || MaxPrice < 0)
            return VoidResult.Failure("Price cannot be negative.");
    
        if (PageNumber < 1)
            return VoidResult.Failure("Page cannot be less than 1.");
            
        if (PageSize < 1)
            return VoidResult.Failure("Page size cannot be less than 1.");
        
        return VoidResult.Success();
    }
}

