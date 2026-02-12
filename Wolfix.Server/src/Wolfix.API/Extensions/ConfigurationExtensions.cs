namespace Wolfix.API.Extensions;

public static class ConfigurationExtensions
{
    public static string GetOrThrow(this IConfiguration configuration, string key)
    {
        return configuration[key] ?? throw new Exception($"Configuration key {key} not found");
    }
}