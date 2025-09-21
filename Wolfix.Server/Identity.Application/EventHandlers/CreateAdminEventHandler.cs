using Admin.IntegrationEvents;
using Identity.Application.Interfaces.Repositories;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Identity.Application.EventHandlers;

public sealed class CreateAdminEventHandler(IAuthStore authStore)
    : IIntegrationEventHandler<CreateAdmin, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateAdmin @event, CancellationToken ct)
    {
        Result<Guid> existingAccountId = await authStore.CheckUserExistsAndHasRole(@event.Email, @event.Password, 
            Roles.Customer, ct);
        
        if (existingAccountId.IsSuccess)
        {
            Guid accountId = existingAccountId.Value;
            
            VoidResult addAdminRoleResult = await authStore.AddAdminRoleAsync(accountId, ct);

            if (addAdminRoleResult.IsFailure)
            {
                return Result<Guid>.Failure(addAdminRoleResult);
            }
            
            return Result<Guid>.Success(accountId);
        }
        
        Result<Guid> createAdminAccountResult = await authStore.RegisterAccountAsync(@event.Email, @event.Password, 
            Roles.Admin, ct);

        if (createAdminAccountResult.IsFailure)
        {
            return createAdminAccountResult;
        }

        Guid createdAccountId = createAdminAccountResult.Value;
        
        return Result<Guid>.Success(createdAccountId);
    }
}