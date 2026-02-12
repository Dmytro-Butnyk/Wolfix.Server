using System.Net;
using System.Text.Json;
using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Domain.Services;
using Catalog.Domain.ValueObjects.AddProduct;
using Catalog.IntegrationEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Core.Endpoints;
using Shared.Core.Exceptions;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.IntegrationEvents;

namespace Catalog.Core.Features.Product;

public static class CreateProduct
{
    public sealed class Request
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public string Status { get; init; }
        public Guid CategoryId { get; init; }
        public Guid SellerId { get; init; }
        public string ContentType { get; init; }
        public IFormFile? Media { get; init; }
    
        [FromForm(Name = "Attributes")]
        public required string AttributesJson { get; init; }

        public List<AddAttributeDto> Attributes =>
            string.IsNullOrEmpty(AttributesJson)
                ? new List<AddAttributeDto>()
                : JsonSerializer.Deserialize<List<AddAttributeDto>>(
                    AttributesJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;
    }
    
    public sealed record AddAttributeDto(
        Guid Id,
        string Value);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/products", Handle)
                .DisableAntiforgery()
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithSummary("Add product")
                .WithTags("Products");
        }

        private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> Handle(
            [FromForm] Request addProductDto,
            [FromServices] ProductDomainService productDomainService,
            [FromServices] EventBus eventBus,
            CancellationToken ct)
        {
            VoidResult checkSellerExistsResult = await eventBus.PublishWithoutResultAsync(
                new CheckSellerExistsForProductAddition(addProductDto.SellerId, addProductDto.CategoryId), ct);

            if (checkSellerExistsResult.IsFailure)
            {
                return TypedResults.BadRequest(checkSellerExistsResult.ErrorMessage);
            }
        
            if (!Enum.TryParse(addProductDto.Status, out ProductStatus productStatus))
            {
                return TypedResults.BadRequest("Invalid status");
            }

            //todo: вместо хардкода указать тут addProductDto.ContentType
            if (!Enum.TryParse("Photo", out BlobResourceType blobResourceType))
            {
                return TypedResults.BadRequest("Invalid blob resource type");
            }

            if (addProductDto.Media is null)
            {
                return TypedResults.BadRequest("Media is null");
            }

            IReadOnlyCollection<AddAttributeValueObject> attributes = addProductDto.Attributes
                .Select(attr => new AddAttributeValueObject(attr.Id, attr.Value))
                .ToList();

            //todo: исправить логику доменного сервиса (сейчас продукт создается внутри него, что не очень хорошо)

            Result<Guid> result = await productDomainService.AddProductAsync(
                addProductDto.Title,
                addProductDto.Description,
                addProductDto.Price,
                productStatus,
                addProductDto.CategoryId,
                addProductDto.SellerId,
                attributes,
                ct
            );

            if (result.IsFailure)
            {
                return result.StatusCode switch
                {
                    HttpStatusCode.BadRequest => TypedResults.BadRequest(result.ErrorMessage),
                    HttpStatusCode.NotFound => TypedResults.NotFound(result.ErrorMessage),
                    _ => throw new UnknownStatusCodeException(
                        nameof(Product),
                        nameof(Handle),
                        result.StatusCode
                    )
                };
            }

            //todo: пофиксить (ажур аккаунт срок истёк)
        
            // VoidResult eventResult = await eventBus.PublishWithoutResultAsync(
            //     new ProductMediaAdded(
            //         result.Value,
            //         new MediaEventDto(blobResourceType, addProductDto.Media, false)),
            //     ct);
            //
            // if (eventResult.IsFailure)
            // {
            //     return eventResult.StatusCode switch
            //     {
            //         HttpStatusCode.BadRequest => TypedResults.BadRequest(eventResult.ErrorMessage),
            //         HttpStatusCode.NotFound => TypedResults.NotFound(eventResult.ErrorMessage),
            //         _ => throw new UnknownStatusCodeException(
            //             nameof(CreateProduct),
            //             nameof(Handle),
            //             eventResult.StatusCode
            //         )
            //     };
            // }

            return TypedResults.NoContent();
        }
    }
}