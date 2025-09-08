using Shared.Domain.Projections;

namespace Catalog.Domain.Projections.Product;

public sealed record ProductShortProjection(
    Guid Id, string Title, double? AverageRating,
    decimal Price, decimal FinalPrice, uint Bonuses, string? MainPhoto): BaseProjection(Id);