using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Extensions;

public static class IdentityResultExtensions
{
    public static string GetErrorMessage(this IdentityResult failedResult)
    {
        if (failedResult.Succeeded) throw new ArgumentException("result succeeded", nameof(failedResult));
        
        return string.Join(Environment.NewLine, failedResult.Errors.Select(e => e.Description));
    }
}