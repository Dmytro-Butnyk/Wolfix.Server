using Shared.Domain.Entities;

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
    
    public static explicit operator CityInfo(City city)
        => new(city.Id, city.Name);
}

public sealed record CityInfo(Guid Id, string Name);