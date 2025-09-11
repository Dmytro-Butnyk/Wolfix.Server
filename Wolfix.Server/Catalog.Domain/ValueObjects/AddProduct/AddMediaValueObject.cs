using Shared.Domain.Enums;

namespace Catalog.Domain.ValueObjects.AddProduct;

public sealed record AddMediaValueObject(
    BlobResourceType ContentType,
    Stream Filestream);
