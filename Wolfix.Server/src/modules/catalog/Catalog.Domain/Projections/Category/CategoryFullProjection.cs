using Shared.Domain.Projections;

namespace Catalog.Domain.Projections.Category;

public sealed record CategoryFullProjection(Guid Id, string Name, string PhotoUrl) : BaseProjection(Id);