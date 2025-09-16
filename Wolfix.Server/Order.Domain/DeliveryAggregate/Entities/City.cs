using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Order.Domain.DeliveryAggregate.Entities;

internal sealed class City : BaseEntity
{
    public string Name { get; private set; }
    
    private readonly List<Department> _departments = [];
    public IReadOnlyCollection<DepartmentInfo> Departments => _departments
        .Select(department => (DepartmentInfo)department)
        .ToList()
        .AsReadOnly();
    
    private readonly List<PostMachine> _postMachines = [];
    public IReadOnlyCollection<PostMachineInfo> PostMachines => _postMachines
        .Select(postMachine => (PostMachineInfo)postMachine)
        .ToList()
        .AsReadOnly();
    
    private City() { }

    private City(string name)
    {
        Name = name;
    }

    public static Result<City> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<City>.Failure($"{nameof(name)} cannot be null or empty");
        }
        
        if (name.Length > 100)
        {
            return Result<City>.Failure($"{nameof(name)} cannot be longer than 100 characters");
        }

        return Result<City>.Success(new(name));
    }
    
    public static explicit operator CityInfo(City city)
        => new(city.Id, city.Name);
}

public sealed record CityInfo(Guid Id, string Name);