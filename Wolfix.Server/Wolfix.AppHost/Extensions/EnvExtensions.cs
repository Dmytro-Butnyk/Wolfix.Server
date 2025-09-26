using DotNetEnv;

namespace Wolfix.AppHost.Extensions;

public static class EnvExtensions
{
    public static void LoadOrThrow(LoadOptions loadOptions)
    {
        if (!File.Exists(".env"))
        {
            throw new Exception(".env file not found");
        }
        
        Env.Load(options: loadOptions);
    }
}