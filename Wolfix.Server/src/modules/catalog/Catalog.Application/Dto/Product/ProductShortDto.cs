using Shared.Application.Dto;

namespace Catalog.Application.Dto.Product;

public sealed record ProductShortDto(Guid Id, string Title, double? AverageRating,
    decimal Price, decimal FinalPrice, uint Bonuses, string? MainPhoto) : BaseDto(Id);