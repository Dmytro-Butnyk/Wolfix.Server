namespace Wolfix.AppHost.Extensions;

public static class ResourceBuilderExtensions
{
    public static IResourceBuilder<T> WithCustomEnvironmentVariables<T>(this IResourceBuilder<T> builder,
        IEnumerable<KeyValuePair<string, string>> envKeyValues) where T : IResourceWithEnvironment
    {
        foreach (var keyValuePair in envKeyValues)
        {
            (string key, string value) = keyValuePair;
            builder.WithEnvironment(key, value);
        }
        
        return builder;
    }
}