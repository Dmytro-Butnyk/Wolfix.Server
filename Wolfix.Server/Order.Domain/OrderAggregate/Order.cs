using Order.Domain.OrderAggregate.Entities;
using Order.Domain.OrderAggregate.Enums;
using Order.Domain.OrderAggregate.ValueObjects;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Order.Domain.OrderAggregate;

public sealed class Order : BaseEntity
{
    public CustomerInfo CustomerInfo { get; private set; }
    
    public Guid CustomerId { get; private set; }
    
    public RecipientInfo RecipientInfo { get; private set; }
    
    public OrderPaymentOption PaymentOption { get; private set; }
    
    public OrderPaymentStatus PaymentStatus { get; private set; }
    
    public DeliveryInfo DeliveryInfo { get; private set; }
    
    public string DeliveryMethodName { get; private set; }
    
    public bool WithBonuses { get; private set; }
    
    public decimal UsedBonusesAmount { get; private set; }

    public decimal Price { get; private set; }
    
    public string? PaymentIntentId { get; private set; } = string.Empty;
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItemInfo> OrderItems => _orderItems
        .Select(oi => (OrderItemInfo)oi)
        .ToList()
        .AsReadOnly();
    
    private Order() { }

    private Order(CustomerInfo customerInfo, Guid customerId, RecipientInfo recipientInfo,
        OrderPaymentOption paymentOption, OrderPaymentStatus paymentStatus,
        string deliveryMethodName, DeliveryInfo deliveryInfo, bool withBonuses,
        decimal usedBonusesAmount, decimal price)
    {
        CustomerInfo = customerInfo;
        CustomerId = customerId;
        RecipientInfo = recipientInfo;
        PaymentOption = paymentOption;
        PaymentStatus = paymentStatus;
        DeliveryMethodName = deliveryMethodName;
        DeliveryInfo = deliveryInfo;
        WithBonuses = withBonuses;
        UsedBonusesAmount = usedBonusesAmount;
        Price = price;
    }
    
    public static Result<Order> Create(string customerFirstName, string customerLastName, string customerMiddleName,
        string customerPhoneNumber, string customerEmail, Guid customerId, string recipientFirstName, string recipientLastName,
        string recipientMiddleName, string recipientPhoneNumber, OrderPaymentOption paymentOption, OrderPaymentStatus paymentStatus,
        string deliveryMethodName, uint? deliveryInfoNumber, string deliveryInfoCity, string deliveryInfoStreet,
        uint deliveryInfoHouseNumber, DeliveryOption deliveryOption,
        bool withBonuses, decimal usedBonusesAmount, decimal price)
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

        if (string.IsNullOrWhiteSpace(deliveryMethodName))
        {
            return Result<Order>.Failure($"{nameof(deliveryMethodName)} cannot be null or empty");
        }

        Result<DeliveryInfo> createDeliveryInfo = DeliveryInfo.Create(deliveryInfoCity, deliveryInfoStreet,
            deliveryInfoHouseNumber, deliveryInfoNumber, deliveryOption);
        
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
            deliveryMethodName, deliveryInfo, withBonuses, usedBonusesAmount, price));
    }

    public VoidResult AddPaymentIntentId(string paymentIntentId)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId))
        {
            return VoidResult.Failure($"{nameof(paymentIntentId)} cannot be null or empty");
        }

        if (PaymentStatus == OrderPaymentStatus.Paid)
        {
            return VoidResult.Failure($"{nameof(paymentIntentId)} cannot be added when order is not pending");
        }
        
        PaymentIntentId = paymentIntentId;
        return VoidResult.Success();
    }

    public VoidResult MarkAsPaid()
    {
        if (PaymentOption == OrderPaymentOption.WhileReceiving)
        {
            return VoidResult.Failure("Order cannot be paid when payment option is while receiving");
        }
        
        if (PaymentStatus != OrderPaymentStatus.Pending)
        {
            return VoidResult.Failure("Order must have Pending status for mark as paid");
        }

        if (PaymentIntentId == null)
        {
            return VoidResult.Failure("Payment intent id cannot be null");
        }
        
        PaymentStatus = OrderPaymentStatus.Paid;
        return VoidResult.Success();
    }

    #region orderItems
    public VoidResult AddOrderItem(Guid productId, string photoUrl, string title, uint quantity, decimal price)
    {
        if (_orderItems.Any(oi => oi.ProductId == productId))
        {
            return VoidResult.Failure($"Order item with product id: {productId} already exist");
        }

        Result<OrderItem> createOrderItemResult = OrderItem.Create(productId, photoUrl, title, quantity, price, this);

        if (createOrderItemResult.IsFailure)
        {
            return VoidResult.Failure(createOrderItemResult);
        }
        
        _orderItems.Add(createOrderItemResult.Value!);
        return VoidResult.Success();
    }
    #endregion
}