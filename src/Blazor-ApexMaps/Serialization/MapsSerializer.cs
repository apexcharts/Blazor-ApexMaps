using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor_ApexMaps.Serialization;

/// <summary>
/// Central System.Text.Json configuration for every payload sent to the apexmaps core:
/// camelCase keys, enums as camelCase strings, nulls omitted.
/// </summary>
/// <remarks>
/// The camelCase policy applies to your own data rows too. A row modelled as
/// <c>class Row { public string Code { get; set; } }</c> arrives at the core as
/// <c>{ "code": ... }</c>, so a series must join on <c>"code"</c> rather than <c>"Code"</c>.
/// Pass a <see cref="Dictionary{TKey, TValue}"/> when you need the key spelled exactly as written.
/// </remarks>
public static class MapsSerializer
{
    /// <summary>Shared serializer options used for every payload sent to the core.</summary>
    public static readonly JsonSerializerOptions Default = Build();

    private static JsonSerializerOptions Build()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            // Dictionary keys are data, not .NET member names: a join key or a GeoJSON property
            // spelled "ISO_A3" has to survive as written, so keys are never rewritten.
            DictionaryKeyPolicy = null,
        };
        // Single-word enum members serialize to their lowercase form (Mercator -> "mercator").
        // Members whose wire form is not a camelCase word (EPSG codes, hyphenated positions)
        // carry a [JsonStringEnumMemberName] attribute, which this converter honours.
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }

    /// <summary>Serializes a value to JSON using <see cref="Default"/>.</summary>
    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Default);
}
