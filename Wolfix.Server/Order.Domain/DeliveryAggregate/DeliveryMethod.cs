using Order.Domain.DeliveryAggregate.Entities;
using Shared.Domain.Entities;

namespace Order.Domain.DeliveryAggregate;

public sealed class DeliveryMethod : BaseEntity
{
    public string Name { get; private set; }
    
    private readonly List<City> _cities = [];
    public IReadOnlyCollection<CityInfo> Cities => _cities
        .Select(city => (CityInfo)city)
        .ToList()
        .AsReadOnly();
}