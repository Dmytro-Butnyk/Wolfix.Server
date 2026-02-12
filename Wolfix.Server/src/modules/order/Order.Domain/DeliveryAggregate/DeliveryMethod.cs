using Order.Domain.DeliveryAggregate.Entities;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Order.Domain.DeliveryAggregate;

public sealed class DeliveryMethod : BaseEntity
{
    public string Name { get; private set; }
    
    private readonly List<City> _cities = [];
    public IReadOnlyCollection<CityInfo> Cities => _cities
        .Select(city => (CityInfo)city)
        .ToList()
        .AsReadOnly();
    
    private DeliveryMethod() { }
    
    private DeliveryMethod(string name)
    {
        Name = name;
    }

    public static Result<DeliveryMethod> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<DeliveryMethod>.Failure($"{nameof(name)} cannot be null or empty");
        }
        
        if (name.Length > 100)
        {
            return Result<DeliveryMethod>.Failure($"{nameof(name)} cannot be longer than 100 characters");
        }
        
        return Result<DeliveryMethod>.Success(new(name));
    }
}