using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Extensions;

public static class IdentityResultExtensions
{
    public static string GetErrorMessage(this IdentityResult result)
    {
        return string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
    }
}