namespace Support.Domain.Projections;

public sealed record SupportRequestAdditionalProperty(string Name, string Value, string Type);

public enum SupportRequestAdditionalPropertyType
{
    Photo,
    Video,
    Document,
    Number,
    String
}