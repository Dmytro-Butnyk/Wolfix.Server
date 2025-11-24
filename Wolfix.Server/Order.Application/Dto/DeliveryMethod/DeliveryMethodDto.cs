namespace Order.Application.Dto.DeliveryMethod;

public sealed record DeliveryMethodDto(Guid Id, string Name, IEnumerable<CityDto> Cities);