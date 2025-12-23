using Shared.Domain.ValueObjects;

namespace Support.Domain.Projections;

public sealed record SupportForAdminProjection(Guid Id, FullName FullName);