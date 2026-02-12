using System.Net;
using Catalog.Domain.Services;
using Catalog.Domain.ValueObjects.FullProductDto;
using Catalog.Infrastructure;
using Catalog.IntegrationEvents;
using Catalog.IntegrationEvents.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Domain.Models;
using Shared.IntegrationEvents;

namespace Catalog.Core.Features.Product;

public static class GetProductFullInfo
{
    public sealed record Response(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        decimal FinalPrice,
        string Status,
        uint Bonuses,
        double? AverageRating,
        IReadOnlyCollection<VariantResponse> Variants,
        IReadOnlyCollection<CategoryResponse> Categories,
        IReadOnlyCollection<MediaResponse> Medias,
        IReadOnlyCollection<AttributeResponse> Attributes,
        SellerResponse Seller
    );

    public sealed record VariantResponse(string Key, string Value);
    public sealed record CategoryResponse(Guid CategoryId, string CategoryName, int Order);
    public sealed record MediaResponse(string Url, string ContentType, bool IsMain);
    public sealed record AttributeResponse(string Key, string Value);
    public sealed record SellerResponse(Guid SellerId, string SellerFullName, string? SellerPhotoUrl);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("product/{productId:guid}", Handle)
                .WithSummary("Get product full info");
        }

        private static async Task<Results<Ok<Response>, BadRequest<string>, NotFound<string>>> Handle(
            [FromRoute] Guid productId,
            [FromServices] CatalogContext db,
            [FromServices] ProductDomainService productDomainService,
            [FromServices] EventBus eventBus,
            CancellationToken ct)
        {
            var product = await db.Products
                .AsNoTracking()
                .Include("_productMedias")
                .Include("_productAttributeValues")
                .Include("_productVariantValues")
                .FirstOrDefaultAsync(p => p.Id == productId, ct);

            if (product is null)
            {
                return TypedResults.NotFound($"Product with id: {productId} not found");
            }

            List<MediaResponse> productMediasDto = product.ProductMedias.Select(pmi =>
                new MediaResponse(pmi.MediaUrl, pmi.MediaType.ToString(), pmi.IsMain)
            ).ToList();

            List<AttributeResponse> productAttributesDto = product.ProductAttributeValues.Select(pav =>
                new AttributeResponse(pav.Key, pav.Value)
            ).ToList();
            
            List<VariantResponse> productVariantsDto = product.ProductVariantValues.Select(pvv =>
                new VariantResponse(pvv.Key, pvv.Value)
            ).ToList();

            Result<IReadOnlyCollection<ProductCategoriesValueObject>> categoriesLineResult =
                await productDomainService.GetCategoriesLineForProduct(product.CategoryId, ct);

            if (categoriesLineResult.IsFailure)
            {
                return categoriesLineResult.StatusCode switch
                {
                    HttpStatusCode.NotFound => TypedResults.NotFound(categoriesLineResult.ErrorMessage),
                    _ => TypedResults.BadRequest(categoriesLineResult.ErrorMessage)
                };
            }

            List<CategoryResponse> categoriesLine = categoriesLineResult.Value!.Select(c =>
                new CategoryResponse(c.CategoryId, c.CategoryName, c.Order)
            ).ToList();

            Result<ProductSellerEventResult> fetchSellerInformationResult =
                await eventBus.PublishWithSingleResultAsync<FetchSellerInformation, ProductSellerEventResult>(
                    new FetchSellerInformation(product.SellerId), ct);

            if (fetchSellerInformationResult.IsFailure)
            {
                return TypedResults.NotFound(fetchSellerInformationResult.ErrorMessage);
            }

            var sellerDto = new SellerResponse(
                fetchSellerInformationResult.Value!.SellerId,
                fetchSellerInformationResult.Value!.SellerFullName,
                fetchSellerInformationResult.Value!.SellerPhotoUrl
            );

            var response = new Response(
                product.Id,
                product.Title,
                product.Description,
                product.Price,
                product.FinalPrice,
                product.Status.ToString(),
                product.Bonuses,
                product.AverageRating,
                productVariantsDto,
                categoriesLine,
                productMediasDto,
                productAttributesDto,
                sellerDto
            );

            return TypedResults.Ok(response);
        }
    }
}