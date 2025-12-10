using Shared.Domain.Entities;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Support.Domain.Enums;

namespace Support.Domain.Entities;

public sealed class SupportRequest : BaseEntity
{
    public Email Email { get; private set; }
    
    public FullName FullName { get; private set; }
    
    public PhoneNumber PhoneNumber { get; private set; }
    
    public BirthDate BirthDate { get; private set; }
    
    public Guid CustomerId { get; private set; }
    
    public string Title { get; private set; }
    
    public Guid? ProductId { get; private set; }
    
    public string RequestContent { get; private set; }

    public SupportRequestStatus Status { get; private set; } = SupportRequestStatus.Pending;

    public Support? ProcessedBy { get; private set; } = null;
    public Guid? SupportId { get; private set; } = null;
    
    public string ResponseContent { get; private set; } = string.Empty;
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public DateTime? ProcessedAt { get; private set; } = null;
    
    private SupportRequest() { }

    private SupportRequest(Email email, FullName fullName, PhoneNumber phoneNumber, BirthDate birthDate, Guid customerId,
        string title, Guid? productId, string requestContent)
    {
        Email = email;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
        CustomerId = customerId;
        Title = title;
        ProductId = productId;
        RequestContent = requestContent;
    }

    public static Result<SupportRequest> Create(string email, string firstName, string lastName, string middleName,
        string phoneNumber, DateOnly birthDate, Guid customerId, string title, string content, Guid? productId = null)
    {
        Result<Email> createEmailResult = Email.Create(email);

        if (createEmailResult.IsFailure)
        {
            return Result<SupportRequest>.Failure(createEmailResult);
        }
        
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

        Result<BirthDate> createBirthDateResult = BirthDate.Create(birthDate);

        if (createBirthDateResult.IsFailure)
        {
            return Result<SupportRequest>.Failure(createBirthDateResult);
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

        SupportRequest supportRequest = new(createEmailResult.Value!, createFullNameResult.Value!,
            createPhoneNumberResult.Value!, createBirthDateResult.Value!, customerId, title, productId, content);
        return Result<SupportRequest>.Success(supportRequest);
    }
    
    public VoidResult Respond(Support support, string responseContent)
    {
        if (Status != SupportRequestStatus.Pending)
        {
            return VoidResult.Failure("Request must be pending to be processed");
        }

        if (ProcessedBy != null || SupportId != null)
        {
            return VoidResult.Failure("Request is already processed by another support");
        }

        if (ResponseContent != string.Empty)
        {
            return VoidResult.Failure("Response content is already set");
        }

        if (ProcessedAt != null)
        {
            return VoidResult.Failure("Request is already processed");
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
        if (Status != SupportRequestStatus.Pending)
        {
            return VoidResult.Failure("Request must be pending to be canceled");
        }

        if (ProcessedBy != null || SupportId != null)
        {
            return VoidResult.Failure("Request is already processed by another support"); 
        }

        if (ProcessedAt != null)
        {
            return VoidResult.Failure("Request is already processed");
        }
        
        Status = SupportRequestStatus.Canceled;
        ProcessedBy = support;
        SupportId = support.Id;
        ProcessedAt = DateTime.UtcNow;
        return VoidResult.Success();       
    }
}