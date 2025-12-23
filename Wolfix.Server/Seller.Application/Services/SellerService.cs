using System.Net;
using Seller.Application.Dto.Seller;
using Seller.Application.Mapping.Seller;
using Seller.Domain.Interfaces;
using Seller.Domain.Projections.Seller;
using Seller.IntegrationEvents;
using Shared.Application.Dto;
using Shared.Domain.Models;
using Shared.IntegrationEvents;

namespace Seller.Application.Services;

public sealed class SellerService(
    ISellerRepository sellerRepository,
    EventBus eventBus)
{
    public async Task<Result<FullNameDto>> ChangeFullNameAsync(Guid sellerId, ChangeFullNameDto request, CancellationToken ct)
    {
        var seller = await sellerRepository.GetByIdAsync(sellerId, ct);

        if (seller is null)
        {
            return Result<FullNameDto>.Failure(
                $"Seller with id: {sellerId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        VoidResult changeFullNameResult = seller.ChangeFullName(request.FirstName, request.LastName, request.MiddleName);

        if (!changeFullNameResult.IsSuccess)
        {
            return Result<FullNameDto>.Failure(changeFullNameResult);
        }

        await sellerRepository.SaveChangesAsync(ct);
        
        FullNameDto dto = new(request.FirstName, request.LastName, request.MiddleName);
        
        return Result<FullNameDto>.Success(dto);
    }

    public async Task<Result<string>> ChangePhoneNumberAsync(Guid sellerId, ChangePhoneNumberDto request, CancellationToken ct)
    {
        var seller = await sellerRepository.GetByIdAsync(sellerId, ct);

        if (seller is null)
        {
            return Result<string>.Failure(
                $"Seller with id: {sellerId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        VoidResult changePhoneNumberResult = seller.ChangePhoneNumber(request.PhoneNumber);

        if (!changePhoneNumberResult.IsSuccess)
        {
            return Result<string>.Failure(changePhoneNumberResult);
        }
        
        await sellerRepository.SaveChangesAsync(ct);
        
        return Result<string>.Success(request.PhoneNumber);
    }

    public async Task<Result<AddressDto>> ChangeAddressAsync(Guid sellerId, ChangeAddressDto request, CancellationToken ct)
    {
        var seller = await sellerRepository.GetByIdAsync(sellerId, ct);

        if (seller is null)
        {
            return Result<AddressDto>.Failure(
                $"Seller with id: {sellerId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        VoidResult changeAddressResult = seller.ChangeAddress(request.City, request.Street,
            request.HouseNumber, request.ApartmentNumber);

        if (!changeAddressResult.IsSuccess)
        {
            return Result<AddressDto>.Failure(changeAddressResult);
        }

        await sellerRepository.SaveChangesAsync(ct);

        AddressDto dto = new(request.City, request.Street, request.HouseNumber, request.ApartmentNumber);
        
        return Result<AddressDto>.Success(dto);
    }

    public async Task<Result<string>> ChangeBirthDateAsync(Guid sellerId, ChangeBirthDateDto request, CancellationToken ct)
    {
        var seller = await sellerRepository.GetByIdAsync(sellerId, ct);

        if (seller is null)
        {
            return Result<string>.Failure(
                $"Seller with id: {sellerId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult changeBirthDateResult = seller.ChangeBirthDate(request.BirthDate);

        if (!changeBirthDateResult.IsSuccess)
        {
            return Result<string>.Failure(changeBirthDateResult);
        }

        await sellerRepository.SaveChangesAsync(ct);
        
        return Result<string>.Success(request.BirthDate.ToString("dd-MM-yyyy"));
    }

    public async Task<Result<SellerDto>> GetProfileInfoAsync(Guid sellerId, CancellationToken ct)
    {
        var seller = await sellerRepository.GetProfileInfoAsync(sellerId, ct);

        if (seller is null)
        {
            return Result<SellerDto>.Failure(
                $"Seller with id: {sellerId} not found",
                HttpStatusCode.NotFound
            );
        }

        return Result<SellerDto>.Success(seller.ToDto());
    }

    public async Task<Result<IReadOnlyCollection<SellerCategoryDto>>> GetAllSellerCategoriesAsync(Guid sellerId, CancellationToken ct)
    {
        var seller = await sellerRepository.GetByIdAsNoTrackingAsync(sellerId, ct, "_sellerCategories");

        if (seller is null)
        {
            return Result<IReadOnlyCollection<SellerCategoryDto>>.Failure(
                $"Seller with id: {sellerId} not found",
                HttpStatusCode.NotFound
            );
        }

        if (seller.SellerCategories.Count == 0)
        {
            return Result<IReadOnlyCollection<SellerCategoryDto>>.Failure(
                $"Seller does not have any categories",
                HttpStatusCode.NotFound
            );
        }

        IReadOnlyCollection<SellerCategoryDto> dto = seller.SellerCategories
            .Select(sc => new SellerCategoryDto(sc.Id, sc.CategoryId, sc.Name))
            .ToList();
        
        return Result<IReadOnlyCollection<SellerCategoryDto>>.Success(dto);
    }

    public async Task<PaginationDto<SellerForAdminDto>> GetForPageAsync(int page, int pageSize, CancellationToken ct)
    {
        int totalCount = await sellerRepository.GetTotalCountAsync(ct);

        if (totalCount == 0)
        {
            return PaginationDto<SellerForAdminDto>.Empty(page);
        }
        
        IReadOnlyCollection<SellerForAdminProjection> projections = await sellerRepository.GetForPageAsync(page, pageSize, ct);
        
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        List<SellerForAdminDto> dto = projections
            .Select(projection => projection.ToAdminDto())
            .ToList();
        
        return new PaginationDto<SellerForAdminDto>(
            CurrentPage: page,
            TotalPages: totalPages,
            TotalItems: totalCount,
            Items: dto
        );
    }

    public async Task<VoidResult> DeleteAsync(Guid sellerId, CancellationToken ct)
    {
        Domain.SellerAggregate.Seller? seller = await sellerRepository.GetByIdAsync(sellerId, ct);

        if (seller is null)
        {
            return VoidResult.Failure(
                $"Seller with id: {sellerId} not found",
                HttpStatusCode.NotFound
            );
        }

        var @event = new DeleteSellerAccount
        {
            AccountId = seller.AccountId
        };

        VoidResult deleteAccountResult = await eventBus.PublishWithoutResultAsync(@event, ct);

        if (deleteAccountResult.IsFailure)
        {
            return deleteAccountResult;
        }
        
        sellerRepository.Delete(seller, ct);
        await sellerRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}