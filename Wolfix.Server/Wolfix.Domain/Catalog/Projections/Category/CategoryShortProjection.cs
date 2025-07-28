using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.Projections.Category;

public sealed record CategoryShortProjection(Guid Id, string Name) : BaseProjection(Id);