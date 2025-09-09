using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Extensions;

public static class IdentityResultExtensions
{
    public static string GetErrorMessage(this IdentityResult result)
        => string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
}