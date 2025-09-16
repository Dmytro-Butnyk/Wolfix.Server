using Order.Domain.OrderAggregate.Entities;
using Order.Domain.OrderAggregate.Enums;
using Order.Domain.OrderAggregate.ValueObjects;
using Shared.Domain.Entities;
using Shared.Domain.Models;

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
    
    internal DeliveryInfo DeliveryInfo { get; private set; }
    
    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItemInfo> OrderItems => _orderItems
        .Select(oi => (OrderItemInfo)oi)
        .ToList()
        .AsReadOnly();
    
    public bool WithBonuses { get; private set; }
    
    public decimal UsedBonusesAmount { get; private set; }

    public decimal Price { get; private set; }
    
    private Order() { }

    private Order(CustomerInfo customerInfo, Guid customerId, RecipientInfo recipientInfo,
        OrderPaymentOption paymentOption, OrderPaymentStatus paymentStatus, Guid deliveryMethodId,
        string deliveryMethodName, DeliveryInfo deliveryInfo, bool withBonuses,
        decimal usedBonusesAmount, decimal price)
    {
        CustomerInfo = customerInfo;
        CustomerId = customerId;
        RecipientInfo = recipientInfo;
        PaymentOption = paymentOption;
        PaymentStatus = paymentStatus;
        DeliveryMethodId = deliveryMethodId;
        DeliveryMethodName = deliveryMethodName;
        DeliveryInfo = deliveryInfo;
        WithBonuses = withBonuses;
        UsedBonusesAmount = usedBonusesAmount;
        Price = price;
    }
    
    public static Result<Order> Create(string customerFirstName, string customerLastName, string customerMiddleName,
        string customerPhoneNumber, string customerEmail, Guid customerId, string recipientFirstName, string recipientLastName,
        string recipientMiddleName, string recipientPhoneNumber, OrderPaymentOption paymentOption, OrderPaymentStatus paymentStatus,
        Guid deliveryMethodId, string deliveryMethodName, uint? deliveryInfoNumber, string deliveryInfoCity,
        string deliveryInfoStreet, uint deliveryInfoHouseNumber, string? deliveryInfoNote, DeliveryOption deliveryOption, bool withBonuses,
        decimal usedBonusesAmount, decimal price)
    {
        if (paymentOption == OrderPaymentOption.WhileReceiving && paymentStatus == OrderPaymentStatus.Paid)
        {
            return Result<Order>.Failure("Order cannot be paid already when payment option is while receiving");
        }
        
        Result<CustomerInfo> createCustomerInfoResult = CustomerInfo.Create(customerFirstName, customerLastName,
            customerMiddleName, customerPhoneNumber, customerEmail);

        if (createCustomerInfoResult.IsFailure)
        {
            return Result<Order>.Failure(createCustomerInfoResult);
        }
        
        var customerInfo = createCustomerInfoResult.Value!;

        if (Guid.Empty == customerId)
        {
            return Result<Order>.Failure($"{nameof(customerInfo)} cannot be empty");
        }

        Result<RecipientInfo> createRecipientInfoResult = RecipientInfo.Create(recipientFirstName, recipientLastName,
            recipientMiddleName, recipientPhoneNumber);

        if (createRecipientInfoResult.IsFailure)
        {
            return Result<Order>.Failure(createRecipientInfoResult);
        }
        
        var recipientInfo = createRecipientInfoResult.Value!;

        if (Guid.Empty == deliveryMethodId)
        {
            return Result<Order>.Failure($"{nameof(deliveryMethodId)} cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(deliveryMethodName))
        {
            return Result<Order>.Failure($"{nameof(deliveryMethodName)} cannot be null or empty");
        }

        Result<DeliveryInfo> createDeliveryInfo = ValueObjects.DeliveryInfo.Create(deliveryInfoCity, deliveryInfoStreet,
            deliveryInfoHouseNumber, deliveryInfoNumber, deliveryInfoNote, deliveryOption);
        
        if (createDeliveryInfo.IsFailure)
        {
            return Result<Order>.Failure(createDeliveryInfo);
        }
        
        var deliveryInfo = createDeliveryInfo.Value!;

        if (withBonuses && usedBonusesAmount == 0)
        {
            return Result<Order>.Failure($"{nameof(usedBonusesAmount)} cannot be 0 when with bonuses is true");
        }
        
        if (!withBonuses && usedBonusesAmount > 0)
        {
            return Result<Order>.Failure($"{nameof(usedBonusesAmount)} cannot be greater than 0 when with bonuses is false");
        }
        
        if (price <= 0)
        {
            return Result<Order>.Failure($"{nameof(price)} must be positive");
        }

        return Result<Order>.Success(new(customerInfo, customerId, recipientInfo, paymentOption, paymentStatus,
            deliveryMethodId, deliveryMethodName, deliveryInfo, withBonuses, usedBonusesAmount, price));
    }
}