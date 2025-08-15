using Media.Domain.BlobAggregate.Enums;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Media.Domain.BlobAggregate;

public sealed class BlobResource : BaseEntity
{
    public string Name { get; private set; }
    public string Url { get; private set; } = String.Empty;
    public BlobResourceType Type { get; private set; }

    private BlobResource()
    {
    }

    private BlobResource(string name, BlobResourceType type)
    {
        Name = name;
        Type = type;
    }

    public static Result<BlobResource> Create(BlobResourceType type)
    {
        if (!Enum.IsDefined(type))
        {
            return Result<BlobResource>.Failure("Invalid blob resource type");
        }

        string name = $"{Guid.NewGuid():N}-{type.ToString().ToLowerInvariant()}";

        BlobResource blobResource = new BlobResource(name, type);

        return Result<BlobResource>.Success(blobResource);
    }

    public VoidResult ChangeName(string name)
    {
        if (IsTextInvalid(name, out string errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        Name = name;
        return VoidResult.Success();
    }

    public VoidResult ChangeUrl(string url)
    {
        if (IsTextInvalid(url, out string errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        Url = url;
        return VoidResult.Success();
    }

    public VoidResult ChangeType(BlobResourceType type)
    {
        if (!Enum.IsDefined(type))
        {
            return VoidResult.Failure("Invalid blob resource type");
        }
        
        Type = type;
        return VoidResult.Success();
    }

    #region Validation

    private static bool IsTextInvalid(string text, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            errorMessage = $"{nameof(text)} is required";
            return true;
        }

        errorMessage = string.Empty;
        return false;
    }

    #endregion
}