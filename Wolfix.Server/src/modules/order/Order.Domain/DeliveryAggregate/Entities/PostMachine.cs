using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Order.Domain.DeliveryAggregate.Entities;

internal sealed class PostMachine : BaseEntity
{
    public uint Number { get; private set; }
    
    public string Street { get; private set; }
    
    public uint HouseNumber { get; private set; }
    
    public string? Note { get; private set; }
    
    public City City { get; private set; }
    public Guid CityId { get; private set; }
    
    private PostMachine() { }

    private PostMachine(uint number, string street, uint houseNumber, string? note, City city)
    {
        Number = number;
        Street = street;
        HouseNumber = houseNumber;
        Note = note;
        City = city;
        CityId = city.Id;
    }

    public static Result<PostMachine> Create(uint number, string street, uint houseNumber, string? note, City city)
    {
        if (number <= 0)
        {
            return Result<PostMachine>.Failure($"{nameof(number)} must be positive");
        }
        
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result<PostMachine>.Failure($"{nameof(street)} cannot be null or empty");
        }
        
        if (houseNumber <= 0)
        {
            return Result<PostMachine>.Failure($"{nameof(houseNumber)} must be positive");
        }

        return Result<PostMachine>.Success(new(number, street, houseNumber, note, city));
    }

    public static explicit operator PostMachineInfo(PostMachine postMachine)
        => new(postMachine.Id, postMachine.Number, postMachine.Street, postMachine.HouseNumber, postMachine.Note);
}

public sealed record PostMachineInfo(Guid Id, uint Number, string Street, uint HouseNumber, string? Note);