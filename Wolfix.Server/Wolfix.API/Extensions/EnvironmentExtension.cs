using DotNetEnv;

namespace Wolfix.API.Extensions;

public static class EnvironmentExtension
{
    public static string GetEnvironmentVariableOrThrow(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);

        if (value is null)
        {
            throw new EnvVariableNotFoundException("Environment variable not found: " + key, key);
        }

        return value;
    }
}