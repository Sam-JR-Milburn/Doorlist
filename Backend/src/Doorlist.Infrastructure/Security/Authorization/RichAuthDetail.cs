namespace Doorlist.Infrastructure.Security.Authorization;

using System.Text.Json.Serialization;

/**
 * RarDetail
 * Lightweight helper for the RFC 9396 Rich Authorisation Request implementation
 */
public record RichAuthDetail(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("locations")] List<string> Locations,
    [property: JsonPropertyName("actions")] List<string> Actions
);