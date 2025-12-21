using Shared.Domain.Entities;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Support.Domain.Enums;

namespace Support.Domain.Entities;

public sealed class SupportRequest : BaseEntity
{
    public FullName FullName { get; private set; }
    
    public PhoneNumber PhoneNumber { get; private set; }
    
    public BirthDate? BirthDate { get; private set; }
    
    public Guid CustomerId { get; private set; }
    
    public string Title { get; private set; }
    
    public Guid? ProductId { get; private set; }
    public SupportRequestCategory Category { get; private set; }
    
    public string RequestContent { get; private set; }

    public SupportRequestStatus Status { get; private set; } = SupportRequestStatus.Pending;

    public Support? ProcessedBy { get; private set; } = null;
    public Guid? SupportId { get; private set; } = null;
    
    public bool IsProcessed
        => Status != SupportRequestStatus.Pending
           && ProcessedBy != null
           && SupportId != null
           && SupportId == ProcessedBy.Id
           && ProcessedAt != null;
    
    public string ResponseContent { get; private set; } = string.Empty;
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public DateTime? ProcessedAt { get; private set; } = null;
    
    private SupportRequest() { }

    private SupportRequest(FullName fullName, PhoneNumber phoneNumber, BirthDate? birthDate, Guid customerId,
        string title, Guid? productId, SupportRequestCategory category, string requestContent)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
        CustomerId = customerId;
        Title = title;
        ProductId = productId;
        Category = category;
        RequestContent = requestContent;
    }

    public static Result<SupportRequest> Create(string firstName, string lastName, string middleName,
        string phoneNumber, DateOnly? birthDate, Guid customerId, string title, string category, string content, Guid? productId = null)
    {
        Result<FullName> createFullNameResult = FullName.Create(firstName, lastName, middleName);

        if (createFullNameResult.IsFailure)
        {
            return Result<SupportRequest>.Failure(createFullNameResult);
        }
        
        Result<PhoneNumber> createPhoneNumberResult = PhoneNumber.Create(phoneNumber);
        
        if (createPhoneNumberResult.IsFailure)
        {
            return Result<SupportRequest>.Failure(createPhoneNumberResult);
        }

        Shared.Domain.ValueObjects.BirthDate? birthDateVO = null;
        
        if (birthDate is not null)
        {
            Result<BirthDate> createBirthDateResult = BirthDate.Create(birthDate.Value);
            
            if (createBirthDateResult.IsFailure)
            {
                return Result<SupportRequest>.Failure(createBirthDateResult);
            }
            
            birthDateVO = createBirthDateResult.Value!;
        }

        if (customerId == Guid.Empty)
        {
            return Result<SupportRequest>.Failure("Customer Id is required");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<SupportRequest>.Failure("Title is required");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return Result<SupportRequest>.Failure("RequestContent is required");
        }

        if (string.IsNullOrWhiteSpace(category) || !Enum.TryParse<SupportRequestCategory>(category, out var categoryValue))
        {
            return Result<SupportRequest>.Failure("Category is required");
        }
        
        SupportRequest supportRequest = new(createFullNameResult.Value!,
            createPhoneNumberResult.Value!, birthDateVO, customerId, title, productId, categoryValue, content);
        return Result<SupportRequest>.Success(supportRequest);
    }
    
    public VoidResult Respond(Support support, string responseContent)
    {
        if (IsProcessed)
        {
            return VoidResult.Failure("Request must be not processed yet to be responded");
        }

        if (ResponseContent != string.Empty)
        {
            return VoidResult.Failure("Response content is already set");
        }

        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return VoidResult.Failure("Response content is required");
        }

        Status = SupportRequestStatus.Processed;
        ProcessedBy = support;
        SupportId = support.Id;
        ResponseContent = responseContent;
        ProcessedAt = DateTime.UtcNow;
        return VoidResult.Success();
    }

    public VoidResult Cancel(Support support)
    {
        if (IsProcessed)
        {
            return VoidResult.Failure("Request must be not processed yet to be canceled");
        }
        
        Status = SupportRequestStatus.Canceled;
        ProcessedBy = support;
        SupportId = support.Id;
        ProcessedAt = DateTime.UtcNow;
        return VoidResult.Success();       
    }
}