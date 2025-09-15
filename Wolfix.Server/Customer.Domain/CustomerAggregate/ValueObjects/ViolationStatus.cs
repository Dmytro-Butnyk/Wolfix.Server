using Customer.Domain.CustomerAggregate.Enums;
using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate.ValueObjects;

internal sealed class ViolationStatus
{
    private const uint MaxViolationsCount = 3;
    
    public uint ViolationsCount { get; private set; }

    public AccountStatus Status { get; private set; }

    private ViolationStatus(uint violationsCount, AccountStatus status)
    {
        ViolationsCount = violationsCount;
        Status = status;
    }

    public static Result<ViolationStatus> Create(uint violationsCount = 0, AccountStatus status = AccountStatus.Active)
    {
        if (violationsCount > MaxViolationsCount)
        {
            return Result<ViolationStatus>.Failure($"{nameof(violationsCount)} cannot be greater than {MaxViolationsCount}");
        }

        if (violationsCount < MaxViolationsCount && status == AccountStatus.Blocked)
        {
            return Result<ViolationStatus>.Failure($"{nameof(status)} cannot be {AccountStatus.Blocked} " +
                                                   $"when {nameof(violationsCount)} is less than {MaxViolationsCount}");
        }

        if (violationsCount == MaxViolationsCount && status == AccountStatus.Active)
        {
            return Result<ViolationStatus>.Failure($"{nameof(status)} cannot be {AccountStatus.Active} " +
                                                   $"when {nameof(violationsCount)} is equal to {MaxViolationsCount}");
        }

        return Result<ViolationStatus>.Success(new(violationsCount, status));
    }

    public Result<ViolationStatus> AddViolation()
    {
        if (Status == AccountStatus.Blocked)
        {
            return Result<ViolationStatus>.Failure("Account is already blocked");
        }
        
        var newCount = ViolationsCount + 1;
        var newStatus = newCount == MaxViolationsCount ? AccountStatus.Blocked : AccountStatus.Active;
        
        return Result<ViolationStatus>.Success(new(newCount, newStatus));
    }
}