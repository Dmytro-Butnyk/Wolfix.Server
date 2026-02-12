using Identity.Application.Interfaces.Repositories;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using Support.IntegrationEvents;

namespace Identity.Application.EventHandlers;

public sealed class CreateSupportEventHandler(IAuthStore authStore)
    : IIntegrationEventHandler<CreateSupport, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateSupport @event, CancellationToken ct)
    {
        Result<Guid> existingAccountId = await authStore.CheckUserExistsAndHasRoleAsync(@event.Email, @event.Password, 
            Roles.Customer, ct);
        
        if (existingAccountId.IsSuccess)
        {
            Guid accountId = existingAccountId.Value;
            
            VoidResult addSupportRoleResult = await authStore.AddRoleAsync(accountId, Roles.Support, ct);

            if (addSupportRoleResult.IsFailure)
            {
                return Result<Guid>.Failure(addSupportRoleResult);
            }
            
            return Result<Guid>.Success(accountId);
        }
        
        Result<Guid> createSupportAccountResult = await authStore.RegisterAccountAsync(@event.Email, @event.Password, 
            Roles.Support, ct);

        if (createSupportAccountResult.IsFailure)
        {
            return createSupportAccountResult;
        }

        Guid createdAccountId = createSupportAccountResult.Value;
        
        return Result<Guid>.Success(createdAccountId);
    }
}