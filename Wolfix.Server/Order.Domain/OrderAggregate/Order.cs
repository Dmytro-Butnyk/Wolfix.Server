using Order.Domain.OrderAggregate.Entities;
using Order.Domain.OrderAggregate.Enums;
using Order.Domain.OrderAggregate.ValueObjects;
using Shared.Domain.Entities;

namespace Order.Domain.OrderAggregate;

public sealed class Order : BaseEntity
{
    internal CustomerInfo CustomerInfo { get; private set; }
    
    public Guid CustomerId { get; private set; }
    
    internal RecipientInfo RecipientInfo { get; private set; }
    
    public OrderPaymentOption PaymentOption { get; private set; }
    
    public OrderPaymentStatus PaymentStatus { get; private set; }
    
    public Guid DeliveryMethodId { get; private set; }
    
    public string DeliveryMethodName { get; private set; }
    
    internal DeliveryAddress DeliveryAddress { get; private set; }
    
    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItemInfo> OrderItems => _orderItems
        .Select(oi => (OrderItemInfo)oi)
        .ToList()
        .AsReadOnly();
    
    public bool WithBonuses { get; private set; }
    
    public decimal? UsedBonusesAmount { get; private set; }

    public decimal Price { get; private set; }
}