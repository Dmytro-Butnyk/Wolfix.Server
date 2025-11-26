namespace Order.Domain.Projections;

public sealed record OrderItemDetailsProjection(Guid Id, string PhotoUrl, string Title, uint Quantity, decimal Price, Guid ProductId);