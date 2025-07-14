using System.Net;
using Wolfix.Domain.Catalog.ProductAggregate.Enums;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.ProductAggregate.Entities;

public sealed class Discount : BaseEntity
{
    public uint Percent { get; private set; }
    
    public DateTime ExpirationDateTime { get; private set; }

    public DiscountStatus Status { get; private set; } = DiscountStatus.Active;
    
    private Discount() { }
    
    private Discount(uint percent, DateTime expirationDateTime)
    {
        Percent = percent;
        ExpirationDateTime = expirationDateTime;
    }

    internal static Result<Discount> Create(uint percent, DateTime expirationDateTime)
    {
        if (percent <= 0)
        {
            return Result<Discount>.Failure($"{nameof(percent)} must be positive");
        }
        
        if (percent > 100)
        {
            return Result<Discount>.Failure($"{nameof(percent)} must be less than 100");
        }

        if (expirationDateTime <= DateTime.UtcNow)
        {
            return Result<Discount>.Failure($"{nameof(expirationDateTime)} must be greater than now");
        }

        var discount = new Discount(percent, expirationDateTime);
        return Result<Discount>.Success(discount, HttpStatusCode.Created);
    }
    
    internal VoidResult SetStatus(DiscountStatus status)
    {
        Status = status;
        return VoidResult.Success();
    }
    
    internal VoidResult SetPercent(uint percent)
    {
        if (percent <= 0 || percent > 100)
        {
            return VoidResult.Failure($"{nameof(percent)} must be positive and less than 100");
        }
        
        Percent = percent;
        return VoidResult.Success();
    }
    
    internal VoidResult SetExpirationDateTime(DateTime expirationDateTime)
    {
        if (expirationDateTime <= DateTime.UtcNow)
        {
            return VoidResult.Failure($"{nameof(expirationDateTime)} must be greater than now");
        }
        
        ExpirationDateTime = expirationDateTime;
        return VoidResult.Success();
    }
}