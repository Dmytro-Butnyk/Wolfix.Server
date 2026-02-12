using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Order.Domain.DeliveryAggregate.Entities;

internal sealed class Department : BaseEntity
{
    public uint Number { get; private set; }
    
    public string Street { get; private set; }
    
    public uint HouseNumber { get; private set; }
    
    public City City { get; private set; }
    public Guid CityId { get; private set; }
    
    private Department() { }

    private Department(uint number, string street, uint houseNumber, City city)
    {
        Number = number;
        Street = street;
        HouseNumber = houseNumber;
        City = city;
        CityId = city.Id;
    }

    public static Result<Department> Create(uint number, string street, uint houseNumber, City city)
    {
        if (number <= 0)
        {
            return Result<Department>.Failure($"{nameof(number)} must be positive");
        }
        
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result<Department>.Failure($"{nameof(street)} cannot be null or empty");
        }
        
        if (houseNumber <= 0)
        {
            return Result<Department>.Failure($"{nameof(houseNumber)} must be positive");
        }

        return Result<Department>.Success(new(number, street, houseNumber, city));
    }
    
    public static explicit operator DepartmentInfo(Department department)
        => new(department.Id, department.Number, department.Street, department.HouseNumber);
}

public sealed record DepartmentInfo(Guid Id, uint Number, string Street, uint HouseNumber);