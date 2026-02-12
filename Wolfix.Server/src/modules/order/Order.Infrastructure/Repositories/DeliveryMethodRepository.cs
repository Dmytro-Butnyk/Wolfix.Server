using Microsoft.EntityFrameworkCore;
using Order.Domain.DeliveryAggregate.Entities;
using Order.Domain.Interfaces.DeliveryMethod;
using Order.Domain.Projections;
using Shared.Infrastructure.Repositories;

namespace Order.Infrastructure.Repositories;

internal sealed class DeliveryMethodRepository(OrderContext context)
    : BaseRepository<OrderContext, Domain.DeliveryAggregate.DeliveryMethod>(context), IDeliveryMethodRepository
{
    private readonly DbSet<Domain.DeliveryAggregate.DeliveryMethod> _deliveryMethods = context.DeliveryMethods;
    
    public async Task<IReadOnlyCollection<DeliveryMethodProjection>> GetDeliveryMethodsAsync(CancellationToken ct)
    {
        return await _deliveryMethods
            .AsNoTracking()
            .Include("_cities")
            .Select(dm => new DeliveryMethodProjection(
                dm.Id,
                dm.Name,
                EF.Property<List<City>>(dm, "_cities")
                    .Select(city => new CityProjection(city.Id, city.Name, EF.Property<List<Department>>(city, "_departments")
                        .Select(dep => new DepartmentProjection(dep.Id, dep.Number, dep.Street, dep.HouseNumber)), 
                        EF.Property<List<PostMachine>>(city, "_postMachines")
                            .Select(pm => new PostMachineProjection(pm.Id, pm.Number, pm.Street, pm.HouseNumber, pm.Note))))))
            .ToListAsync(ct);
    }
}