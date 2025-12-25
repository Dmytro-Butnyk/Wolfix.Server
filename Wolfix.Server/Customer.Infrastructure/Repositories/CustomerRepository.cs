using Customer.Domain.CustomerAggregate.Entities;
using Customer.Domain.CustomerAggregate.Enums;
using Customer.Domain.Interfaces;
using Customer.Domain.Projections;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace Customer.Infrastructure.Repositories;

public sealed class CustomerRepository(CustomerContext context)
    : BaseRepository<CustomerContext, Customer.Domain.CustomerAggregate.Customer>(context), ICustomerRepository
{
    private readonly DbSet<Customer.Domain.CustomerAggregate.Customer> _customers = context.Customers;
    
    public async Task<IReadOnlyCollection<FavoriteItemProjection>> GetFavoriteItemsAsync(Guid customerId, CancellationToken ct)
    {
        return await _customers
            .AsNoTracking()
            .Include("_favoriteItems")
            .Where(customer => customer.Id == customerId && customer.ViolationStatus.Status == AccountStatus.Active)
            .SelectMany(customer => EF.Property<List<FavoriteItem>>(customer, "_favoriteItems"))
            .Select(fi => new FavoriteItemProjection(
                fi.Id,
                fi.PhotoUrl,
                fi.Title,
                fi.AverageRating,
                fi.Price,
                fi.FinalPrice,
                fi.Bonuses,
                fi.CustomerId))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<CartItemProjection>> GetCartItemsAsync(Guid customerId, CancellationToken ct)
    {
        return await _customers
            .AsNoTracking()
            .Include("_cartItems")
            .Where(customer => customer.Id == customerId && customer.ViolationStatus.Status == AccountStatus.Active)
            .SelectMany(customer => EF.Property<List<CartItem>>(customer, "_cartItems"))
            .Select(cartItem => new CartItemProjection(
                cartItem.Id,
                cartItem.ProductId,
                cartItem.SellerId,
                cartItem.CustomerId,
                cartItem.PhotoUrl,
                cartItem.Title,
                cartItem.PriceWithDiscount))
            .ToListAsync(ct);
    }

    public async Task<Guid?> GetIdByAccountIdAsync(Guid accountId, CancellationToken ct)
    {
        return await _customers
            .AsNoTracking()
            .Where(customer => customer.AccountId == accountId && customer.ViolationStatus.Status == AccountStatus.Active)
            .Select(customer => customer.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<CustomerProjection?> GetProfileInfoAsync(Guid customerId, CancellationToken ct)
    {
        return await _customers
            .AsNoTracking()
            .Where(customer => customer.Id == customerId && customer.ViolationStatus.Status == AccountStatus.Active)
            .Select(customer => new CustomerProjection(
                customer.Id,
                customer.PhotoUrl,
                customer.FullName,
                customer.PhoneNumber == null ? null : customer.PhoneNumber.Value,
                customer.Address,
                customer.BirthDate == null ? null : customer.BirthDate.Value,
                customer.BonusesAmount))
            .FirstOrDefaultAsync(ct);
    }
}