using System.Net;
using Catalog.Domain.ProductAggregate.Enums;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Catalog.Domain.ProductAggregate.Entities;

internal sealed class Discount : BaseEntity
{
    public uint Percent { get; private set; }
    
    public DateTime ExpirationDateTime { get; private set; }

    public DiscountStatus Status { get; private set; } = DiscountStatus.Active;
    
    public Product Product { get; private set; }
    
    public Guid ProductId { get; private set; }
    
    private Discount() { }
    
    private Discount(uint percent, DateTime expirationDateTime, Product product)
    {
        Percent = percent;
        ExpirationDateTime = expirationDateTime;
        Product = product;
        ProductId = product.Id;
    }

    internal static Result<Discount> Create(uint percent, DateTime expirationDateTime, Product product)
    {
        if (IsPercentInvalid(percent, out var percentErrorMessage))
        {
            return Result<Discount>.Failure(percentErrorMessage);
        }

        if (IsExpirationDateTimeInvalid(expirationDateTime, out var expirationDateTimeErrorMessage))
        {
            return Result<Discount>.Failure(expirationDateTimeErrorMessage);
        }

        var discount = new Discount(percent, expirationDateTime, product);
        return Result<Discount>.Success(discount, HttpStatusCode.Created);
    }
    
    internal VoidResult SetStatus(DiscountStatus status)
    {
        Status = status;
        return VoidResult.Success();
    }
    
    internal VoidResult SetPercent(uint percent)
    {
        if (IsPercentInvalid(percent, out var errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        Percent = percent;
        return VoidResult.Success();
    }
    
    internal VoidResult SetExpirationDateTime(DateTime expirationDateTime)
    {
        if (IsExpirationDateTimeInvalid(expirationDateTime, out var errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        ExpirationDateTime = expirationDateTime;
        return VoidResult.Success();
    }

    #region validation
    private static bool IsPercentInvalid(uint percent, out string errorMessage)
    {
        if (percent <= 0)
        {
            errorMessage = $"{nameof(percent)} must be positive";
            return true;
        }
        
        if (percent > 100)
        {
            errorMessage = $"{nameof(percent)} must be less than 100";
            return true;
        }

        errorMessage = string.Empty;
        return false;
    }

    private static bool IsExpirationDateTimeInvalid(DateTime expirationDateTime, out string errorMessage)
    {
        if (expirationDateTime <= DateTime.UtcNow)
        {
            errorMessage = $"{nameof(expirationDateTime)} must be greater than now";
            return true;
        }
        
        errorMessage = string.Empty;
        return false;
    }
    #endregion
    
    public static explicit operator DiscountInfo(Discount discount)
        => new(discount.Id, discount.Percent, discount.ExpirationDateTime, discount.Status);
}

public record DiscountInfo(Guid Id, uint Percent, DateTime ExpirationDateTime, DiscountStatus Status);