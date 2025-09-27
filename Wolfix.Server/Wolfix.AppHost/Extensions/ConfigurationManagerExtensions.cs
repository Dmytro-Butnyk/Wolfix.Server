using Microsoft.Extensions.Configuration;

namespace Wolfix.AppHost.Extensions;

public static class ConfigurationManagerExtensions
{
    public static string GetOrThrow(this ConfigurationManager configuration, string key)
    {
        return configuration[key] ?? throw new Exception($"Configuration key {key} not found");
    }
}