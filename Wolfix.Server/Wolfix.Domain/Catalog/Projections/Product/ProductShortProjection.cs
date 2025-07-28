using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.Projections.Product;

public sealed record ProductShortProjection(
    Guid Id, string Title, double? AverageRating,
    decimal Price, decimal FinalPrice, uint Bonuses): BaseProjection(Id);