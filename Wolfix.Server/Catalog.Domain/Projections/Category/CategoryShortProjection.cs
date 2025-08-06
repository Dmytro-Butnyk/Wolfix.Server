using Shared.Domain.Projections;

namespace Catalog.Domain.Projections.Category;

public sealed record CategoryShortProjection(Guid Id, string Name) : BaseProjection(Id);