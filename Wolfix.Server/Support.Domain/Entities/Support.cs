using Shared.Domain.Entities;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;

namespace Support.Domain.Entities;

public sealed class Support : BaseEntity
{
    public FullName FullName { get; private set; }
    
    public Guid AccountId { get; private set; }

    public List<SupportRequest> SupportRequests = [];
    
    private Support() { }

    private Support(Guid accountId, FullName fullName)
    {
        FullName = fullName;
        AccountId = accountId;
    }

    public static Result<Support> Create(Guid accountId, string firstName, string lastName, string middleName)
    {
        if (accountId == Guid.Empty)
        {
            return Result<Support>.Failure("Account id cannot be empty");
        }

        Result<FullName> createFullNameResult = FullName.Create(firstName, lastName, middleName);

        if (createFullNameResult.IsFailure)
        {
            return Result<Support>.Failure(createFullNameResult);
        }

        Support support = new(accountId, createFullNameResult.Value!);
        return Result<Support>.Success(support);
    }
}