using System.Net;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.AttributeAggregate;

public sealed class Attribute : BaseEntity
{
    public Guid CategoryId { get; private set; }
    public string Key { get; private set; }
    
    private Attribute() { }
    
    private Attribute(Guid categoryId, string key)
    {
        CategoryId = categoryId;
        Key = key;
    }

    public static Result<Attribute> Create(Guid categoryId, string key)
    {
        if (Guid.Empty == categoryId)
        {
            return Result<Attribute>.Failure($"{nameof(categoryId)} is required");
        }
        
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<Attribute>.Failure($"{nameof(key)} is required");
        }

        var attribute = new Attribute(categoryId, key);
        return Result<Attribute>.Success(attribute, HttpStatusCode.Created);
    }
}