namespace Order.Domain.Projections;

public sealed record DeliveryMethodProjection(Guid Id, string Name, IEnumerable<CityProjection> Cities);