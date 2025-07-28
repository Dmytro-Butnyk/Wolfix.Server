using Wolfix.Application.Shared;

namespace Wolfix.Application.Catalog.Dto.Product;

public sealed record ProductShortDto(Guid Id, string Title, double? AverageRating,
    decimal Price, decimal FinalPrice, uint Bonuses) : BaseDto(Id);