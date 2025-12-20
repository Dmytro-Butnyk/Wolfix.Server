using Admin.Application.Dto.Requests;
using Admin.Domain.Interfaces;
using Admin.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using AdminAggregate = Admin.Domain.AdminAggregate.Admin;

namespace Admin.Application.Services;

public sealed class AdminService(
    IAdminRepository adminRepository,
    EventBus eventBus)
{
    public async Task<VoidResult> CreateAdminAsync(CreateAdminDto request, CancellationToken ct)
    {
        var @event = new CreateAdmin
        {
            Email = request.Email,
            Password = request.Password
        };
        
        Result<Guid> createAccountResult = await eventBus.PublishWithSingleResultAsync<CreateAdmin, Guid>(@event, ct);

        if (createAccountResult.IsFailure)
        {
            return VoidResult.Failure(createAccountResult);
        }

        Guid accountId = createAccountResult.Value;

        Result<AdminAggregate> createAdminResult = AdminAggregate.Create(accountId, request.FirstName, request.LastName,
            request.MiddleName, request.PhoneNumber);

        if (createAdminResult.IsFailure)
        {
            return VoidResult.Failure(createAdminResult);
        }

        AdminAggregate admin = createAdminResult.Value!;
        
        await adminRepository.AddAsync(admin, ct);
        await adminRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}