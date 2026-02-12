using System.Net;
using Admin.Application.Dto.Requests;
using Admin.Application.Dto.Responses;
using Admin.Application.Mapping;
using Admin.Domain.Interfaces;
using Admin.Domain.Projections;
using Admin.IntegrationEvents;
using Shared.Application.Dto;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using AdminAggregate = Admin.Domain.AdminAggregate.Admin;

namespace Admin.Application.Services;

public sealed class AdminService(
    IAdminRepository adminRepository,
    EventBus eventBus)
{
    public async Task<VoidResult> CreateAsync(CreateAdminDto request, CancellationToken ct)
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

    public async Task<PaginationDto<BasicAdminDto>> GetForPageAsync(int page, int pageSize, CancellationToken ct)
    {
        int totalCount = await adminRepository.GetBasicAdminsTotalCountAsync(ct);

        if (totalCount == 0)
        {
            return PaginationDto<BasicAdminDto>.Empty(page);
        }

        IReadOnlyCollection<BasicAdminProjection> projections = await adminRepository.GetForPageAsync(page, pageSize, ct);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        List<BasicAdminDto> dto = projections
            .Select(projection => projection.ToDto())
            .ToList();
        
        return new PaginationDto<BasicAdminDto>(page, totalPages, totalCount, dto);
    }

    public async Task<VoidResult> DeleteAsync(Guid adminId, CancellationToken ct)
    {
        AdminAggregate? admin = await adminRepository.GetByIdAsync(adminId, ct);

        if (admin is null)
        {
            return VoidResult.Failure(
                $"Admin with id: {adminId} not found",
                HttpStatusCode.NotFound
            );
        }

        var @event = new DeleteAdminAccount
        {
            AccountId = admin.AccountId
        };
        
        VoidResult deleteAccountResult = await eventBus.PublishWithoutResultAsync(@event, ct);

        if (deleteAccountResult.IsFailure)
        {
            return deleteAccountResult;
        }
        
        adminRepository.Delete(admin, ct);
        await adminRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}