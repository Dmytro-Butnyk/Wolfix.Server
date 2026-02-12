using Shared.Domain.ValueObjects;

namespace Admin.Domain.Projections;

public sealed record BasicAdminProjection(Guid Id, FullName FullName, PhoneNumber PhoneNumber);