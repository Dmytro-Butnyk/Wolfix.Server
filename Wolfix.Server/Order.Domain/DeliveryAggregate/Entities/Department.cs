using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Order.Domain.DeliveryAggregate.Entities;

internal sealed class Department : BaseEntity
{
    public uint Number { get; private set; }
    
    public string Street { get; private set; }
    
    public uint HouseNumber { get; private set; }
    
    public City City { get; private set; }
    public Guid CityId { get; private set; }
    
    public static explicit operator DepartmentInfo(Department department)
        => new(department.Id, department.Number, department.Street, department.HouseNumber);
}

public sealed record DepartmentInfo(Guid Id, uint Number, string Street, uint HouseNumber);