using Shared.Domain.Projections;

namespace Customer.Domain.Projections;

public sealed record CartItemProjection(Guid Id, Guid ProductId, Guid CustomerId, string PhotoUrl, string Title, decimal Price): BaseProjection(Id);