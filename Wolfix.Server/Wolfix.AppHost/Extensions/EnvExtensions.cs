using DotNetEnv;

namespace Wolfix.AppHost.Extensions;

public static class EnvExtensions
{
    public static IEnumerable<KeyValuePair<string, string>> LoadOrThrow(LoadOptions loadOptions)
    {
        if (!File.Exists(".env"))
        {
            throw new Exception(".env file not found");
        }
        
        return Env.Load(options: loadOptions);
    }
}