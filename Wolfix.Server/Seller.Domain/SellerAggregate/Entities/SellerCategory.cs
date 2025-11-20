using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Seller.Domain.SellerAggregate.Entities;

internal sealed class SellerCategory : BaseEntity
{
    public Guid CategoryId { get; private set; }
    
    public string Name { get; private set; }
    
    public Seller Seller { get; private set; }
    public Guid SellerId { get; private set; }

    private SellerCategory() { }

    private SellerCategory(Seller seller, Guid categoryId, string name)
    {
        Seller = seller;
        SellerId = seller.Id;
        CategoryId = categoryId;
        Name = name;
    }

    public static Result<SellerCategory> Create(Seller seller, Guid categoryId, string name)
    {
        if (categoryId == Guid.Empty)
        {
            return Result<SellerCategory>.Failure($"{nameof(categoryId)} cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<SellerCategory>.Failure($"{nameof(name)} cannot be null or empty");
        }

        SellerCategory sellerCategory = new(seller, categoryId, name);
        return Result<SellerCategory>.Success(sellerCategory);
    }
    
    public static explicit operator SellerCategoryInfo(SellerCategory sellerCategory)
        => new(sellerCategory.Id, sellerCategory.CategoryId, sellerCategory.Name, sellerCategory.SellerId);
}

public sealed record SellerCategoryInfo(Guid Id, Guid CategoryId, string Name, Guid SellerId);