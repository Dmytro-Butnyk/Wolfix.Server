using Wolfix.Domain.Catalog.Projections.Shared;

namespace Wolfix.Domain.Catalog.Projections;

public sealed record CategoryShortProjection(Guid Id, string Name) : BaseProjection(Id);