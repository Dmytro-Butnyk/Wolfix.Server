using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Support.Domain.Enums;
using Support.Domain.Extensions;

namespace Support.Domain.Entities.SupportRequests;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(OrderIssueSupportRequest))]
[BsonKnownTypes(typeof(GeneralSupportRequest))]
[BsonKnownTypes(typeof(BugOrErrorSupportRequest))]
public abstract class BaseSupportRequest
{
    [BsonId]
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    
    public FullName FullName { get; private set; }
    
    public PhoneNumber PhoneNumber { get; private set; }
    
    public BirthDate? BirthDate { get; private set; }
    
    public Guid CustomerId { get; private set; }
    
    [BsonRepresentation(BsonType.String)]
    public SupportRequestCategory Category { get; private set; }
    
    public string RequestContent { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public SupportRequestStatus Status { get; private set; } = SupportRequestStatus.Pending;
    
    public Guid? SupportId { get; private set; } = null;
    
    [BsonIgnore]
    public bool IsProcessed
        => Status != SupportRequestStatus.Pending
           && SupportId != null
           && ProcessedAt != null;
    
    public string ResponseContent { get; private set; } = string.Empty;
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public DateTime? ProcessedAt { get; private set; } = null;
    
    [BsonExtraElements]
    public Dictionary<string, object> ExtraElements { get; init; } = [];
    
    protected BaseSupportRequest() { }
    
    protected BaseSupportRequest(FullName fullName, PhoneNumber phoneNumber, BirthDate? birthDate, Guid customerId, 
        SupportRequestCategory category, string requestContent, Dictionary<string, object> extraElements)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
        CustomerId = customerId;
        Category = category;
        RequestContent = requestContent;
        ExtraElements = extraElements.ToDictionaryFromJson();
    }
    
    // public static Result<BaseSupportRequest> Create(string firstName, string lastName, string middleName, string phoneNumber,
    //     DateOnly? birthDate, Guid customerId, string category, string content, Dictionary<string, object> extraElements)
    // {
    //     Result<FullName> createFullNameResult = FullName.Create(firstName, lastName, middleName);
    //
    //     if (createFullNameResult.IsFailure)
    //     {
    //         return Result<BaseSupportRequest>.Failure(createFullNameResult);
    //     }
    //     
    //     Result<PhoneNumber> createPhoneNumberResult = PhoneNumber.Create(phoneNumber);
    //     
    //     if (createPhoneNumberResult.IsFailure)
    //     {
    //         return Result<BaseSupportRequest>.Failure(createPhoneNumberResult);
    //     }
    //
    //     BirthDate? birthDateVo = null;
    //     
    //     if (birthDate is not null)
    //     {
    //         Result<BirthDate> createBirthDateResult = BirthDate.Create(birthDate.Value);
    //         
    //         if (createBirthDateResult.IsFailure)
    //         {
    //             return Result<BaseSupportRequest>.Failure(createBirthDateResult);
    //         }
    //         
    //         birthDateVo = createBirthDateResult.Value!;
    //     }
    //
    //     if (customerId == Guid.Empty)
    //     {
    //         return Result<BaseSupportRequest>.Failure("Customer Id is required");
    //     }
    //
    //     if (string.IsNullOrWhiteSpace(content))
    //     {
    //         return Result<BaseSupportRequest>.Failure("RequestContent is required");
    //     }
    //
    //     if (string.IsNullOrWhiteSpace(category) || !Enum.TryParse<SupportRequestCategory>(category, out var categoryValue))
    //     {
    //         return Result<BaseSupportRequest>.Failure("Category is required");
    //     }
    //
    //     BaseSupportRequest supportRequest = new(createFullNameResult.Value!,
    //         createPhoneNumberResult.Value!, birthDateVo, customerId, categoryValue, content, extraElements);
    //     return Result<BaseSupportRequest>.Success(supportRequest);
    // }

    public static Result<SupportRequestCreateData> ValidateCreateData(string firstName, string lastName, string middleName, string phoneNumber,
        DateOnly? birthDate, Guid customerId, string category, string content)
    {
        Result<FullName> createFullNameResult = FullName.Create(firstName, lastName, middleName);

        if (createFullNameResult.IsFailure)
        {
            return Result<SupportRequestCreateData>.Failure(createFullNameResult);
        }
        
        Result<PhoneNumber> createPhoneNumberResult = PhoneNumber.Create(phoneNumber);
        
        if (createPhoneNumberResult.IsFailure)
        {
            return Result<SupportRequestCreateData>.Failure(createPhoneNumberResult);
        }

        BirthDate? birthDateVo = null;
        
        if (birthDate is not null)
        {
            Result<BirthDate> createBirthDateResult = BirthDate.Create(birthDate.Value);
            
            if (createBirthDateResult.IsFailure)
            {
                return Result<SupportRequestCreateData>.Failure(createBirthDateResult);
            }
            
            birthDateVo = createBirthDateResult.Value!;
        }

        if (customerId == Guid.Empty)
        {
            return Result<SupportRequestCreateData>.Failure("Customer Id is required");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return Result<SupportRequestCreateData>.Failure("RequestContent is required");
        }

        if (string.IsNullOrWhiteSpace(category) || !Enum.TryParse<SupportRequestCategory>(category, out var categoryValue))
        {
            return Result<SupportRequestCreateData>.Failure("Category is required");
        }

        SupportRequestCreateData data = new(createFullNameResult.Value!, createPhoneNumberResult.Value!, birthDateVo, categoryValue);
        return Result<SupportRequestCreateData>.Success(data);
    }
    
    public VoidResult Respond(Guid supportId, string responseContent)
    {
        if (IsProcessed)
        {
            return VoidResult.Failure("Request must be not processed yet to be responded");
        }

        if (ResponseContent != string.Empty)
        {
            return VoidResult.Failure("Response content is already set");
        }

        if (supportId == Guid.Empty)
        {
            return VoidResult.Failure("Support id is required");
        }

        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return VoidResult.Failure("Response content is required");
        }

        Status = SupportRequestStatus.Processed;
        SupportId = supportId;
        ResponseContent = responseContent;
        ProcessedAt = DateTime.UtcNow;
        return VoidResult.Success();
    }

    public VoidResult Cancel(Guid supportId)
    {
        if (IsProcessed)
        {
            return VoidResult.Failure("Request must be not processed yet to be canceled");
        }

        if (supportId == Guid.Empty)
        {
            return VoidResult.Failure("Support id is required");
        }
        
        Status = SupportRequestStatus.Canceled;
        SupportId = supportId;
        ProcessedAt = DateTime.UtcNow;
        return VoidResult.Success();       
    }
}

public sealed record SupportRequestCreateData(FullName FullName, PhoneNumber PhoneNumber, BirthDate? BirthDate, SupportRequestCategory Category);