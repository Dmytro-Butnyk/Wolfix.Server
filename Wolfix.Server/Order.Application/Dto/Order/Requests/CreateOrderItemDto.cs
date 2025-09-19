namespace Order.Application.Dto.Order.Requests;

//todo: тут должно брать только айди продукта или вообще айди cartItem а уже через события будут браться все нужные данные
public sealed record CreateOrderItemDto(Guid ProductId, string PhotoUrl, string Title, uint Quantity, decimal Price);