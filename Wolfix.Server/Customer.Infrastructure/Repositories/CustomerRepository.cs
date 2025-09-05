using Customer.Domain.CustomerAggregate.Entities;
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
            .Where(customer => customer.Id == customerId)
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
            .Where(customer => customer.Id == customerId)
            .SelectMany(customer => EF.Property<List<CartItem>>(customer, "_cartItems"))
            .Select(fi => new CartItemProjection(
                fi.Id,
                fi.CustomerId,
                fi.PhotoUrl,
                fi.Title,
                fi.PriceWithDiscount))
            .ToListAsync(ct);
    }
}