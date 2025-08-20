using System.ComponentModel.DataAnnotations;

namespace Media.Application.Options;

public sealed class AzureBlobContainersNames
{
    [Required]
    public string Photos { get; set; }
    [Required]
    public string Videos { get; set; }
}