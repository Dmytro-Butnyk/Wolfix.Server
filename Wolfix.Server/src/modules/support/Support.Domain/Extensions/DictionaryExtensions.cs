namespace Support.Domain.Extensions;

public static class DictionaryExtensions
{
    private static object? ToNative(this object? obj)
    {
        if (obj is System.Text.Json.JsonElement element)
        {
            return element.ValueKind switch
            {
                System.Text.Json.JsonValueKind.String => element.GetString(),
                System.Text.Json.JsonValueKind.Number => element.TryGetInt32(out int i) ? i : 
                    element.TryGetInt64(out long l) ? l : 
                    element.GetDouble(),
                System.Text.Json.JsonValueKind.True => true,
                System.Text.Json.JsonValueKind.False => false,
                System.Text.Json.JsonValueKind.Null => null,
                _ => element.ToString() 
            };
        }
        return obj;
    }
    
    public static Dictionary<string, object> ToDictionaryFromJson(this Dictionary<string, object> dictionary)
        => dictionary.ToDictionary(
            k => k.Key, 
            v => v.Value.ToNative() ?? ""
        );
}