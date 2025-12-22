using System.Net;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Support.Application.Dto;
using Support.Domain.Interfaces;
using Support.IntegrationEvents;
using SupportEntity = Support.Domain.Entities.Support;

namespace Support.Application.Services;

public sealed class SupportService(
    ISupportRepository supportRepository,
    EventBus eventBus)
{
    public async Task<VoidResult> CreateSupportAsync(CreateSupportDto request, CancellationToken ct)
    {
        var @event = new CreateSupport
        {
            Email = request.Email,
            Password = request.Password
        };

        Result<Guid> createAccountResult = await eventBus.PublishWithSingleResultAsync<CreateSupport, Guid>(@event, ct);

        if (createAccountResult.IsFailure)
        {
            return VoidResult.Failure(createAccountResult);
        }

        Guid accountId = createAccountResult.Value;
        
        bool isAlreadyExist = await supportRepository.IsExistAsync(request.FirstName, request.LastName, request.MiddleName, ct);

        if (isAlreadyExist)
        {
            return VoidResult.Failure(
                $"Support with FirstName: {request.FirstName} + LastName: {request.LastName} + MiddleName: {request.MiddleName} already exist",
                HttpStatusCode.Conflict
            );
        }

        Result<SupportEntity> createSupportResult = SupportEntity.Create(
            accountId,
            request.FirstName,
            request.LastName,
            request.MiddleName
        );

        if (createSupportResult.IsFailure)
        {
            return VoidResult.Failure(createSupportResult);
        }

        await supportRepository.AddAsync(createSupportResult.Value!, ct);
        await supportRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}