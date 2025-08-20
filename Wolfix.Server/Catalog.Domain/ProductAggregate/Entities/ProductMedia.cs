using System.Net;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using Shared.Domain.Models;

namespace Catalog.Domain.ProductAggregate.Entities;

internal sealed class ProductMedia : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; }
    public Guid MediaId { get; private set; }
    public BlobResourceType MediaType { get; private set; }
    public string MediaUrl { get; private set; }
    
    private ProductMedia() { }

    private ProductMedia(
        Product product,
        Guid mediaId,
        BlobResourceType mediaType,
        string mediaUrl)
    {
        ProductId = product.Id;
        Product = product;
        MediaId = mediaId;
        MediaType = mediaType;
        MediaUrl = mediaUrl;
    }

    internal static Result<ProductMedia> Create(
        Product product,
        Guid mediaId,
        BlobResourceType mediaType,
        string mediaUrl)
    {
        ProductMedia productMedia = new ProductMedia(
            product,
            mediaId,
            mediaType,
            mediaUrl);
        
        return Result<ProductMedia>.Success(productMedia, HttpStatusCode.Created);
    }
    
    public static explicit operator ProductMediaInfo(ProductMedia productMedia)
    {
        return new ProductMediaInfo(
            productMedia.Id,
            productMedia.MediaId,
            productMedia.MediaType,
            productMedia.MediaUrl);
    }
}

public record ProductMediaInfo(
    Guid Id,
    Guid MediaId,
    BlobResourceType MediaType,
    string MediaUrl);