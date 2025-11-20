using Admin.Domain.AdminAggregate.Enums;
using Shared.Domain.Entities;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;

namespace Admin.Domain.AdminAggregate;

public sealed class Admin : BaseEntity
{
    public Guid AccountId { get; private set; }
    
    public FullName FullName { get; private set; }
    
    public PhoneNumber PhoneNumber { get; private set; }
    //todo: возможно ещё добавить сущность/таблицу SellerCategoryApplication
    // для того чтобы уже для существующих продавцов подавать заявки на новую категорию для продаж
    public AdminType Type { get; private set; } = AdminType.Basic;
    
    private Admin() { }

    private Admin(Guid accountId, FullName fullName, PhoneNumber phoneNumber)
    {
        AccountId = accountId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
    }

    public static Result<Admin> Create(Guid accountId, string firstName, string lastName, string middleName, string phoneNumber)
    {
        if (accountId == Guid.Empty)
        {
            return Result<Admin>.Failure($"{nameof(accountId)} cannot be empty");
        }

        Result<FullName> createFullNameResult = FullName.Create(firstName, lastName, middleName);

        if (createFullNameResult.IsFailure)
        {
            return Result<Admin>.Failure(createFullNameResult);
        }

        FullName fullName = createFullNameResult.Value!;

        Result<PhoneNumber> createPhoneNumberResult = PhoneNumber.Create(phoneNumber);

        if (createPhoneNumberResult.IsFailure)
        {
            return Result<Admin>.Failure(createPhoneNumberResult);
        }

        PhoneNumber createdPhoneNumber = createPhoneNumberResult.Value!;

        return Result<Admin>.Success(new(accountId, fullName, createdPhoneNumber));
    }
}