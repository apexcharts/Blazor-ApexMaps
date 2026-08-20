using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor_ApexMaps.Models;

// Point and route rows are modelled, because their shape is fixed by the series: a bubble needs a
// position or a join key, an arc needs two endpoints. Choropleth rows are not, because their shape
// is yours: whatever the join key is called plus whatever the value is called. Every row type here
// keeps an extension bag, so extra columns survive to tooltips, labels and accessors.

/// <summary>
/// A bubble row: an explicit position, or a join key resolved to a feature's centroid.
/// </summary>
public class BubbleDatum
{
    /// <summary>Longitude in degrees.</summary>
    public double? Lon { get; set; }

    /// <summary>Latitude in degrees.</summary>
    public double? Lat { get; set; }

    /// <summary>Longitude, for data that came from a <c>[lat, lng]</c> world.</summary>
    public double? Lng { get; set; }

    /// <summary>The value the bubble's area carries.</summary>
    public double? Value { get; set; }

    /// <summary>Display name, used in tooltips and labels.</summary>
    public string? Name { get; set; }

    /// <summary>Anything else on the row: join keys, a second encoding, tooltip material.</summary>
    [JsonExtensionData]
    public Dictionary<string, object?>? Extra { get; set; }
}

/// <summary>
/// A marker row: an explicit position, or a join key resolved to a feature's centroid.
/// </summary>
public class MarkerDatum
{
    /// <summary>Longitude in degrees.</summary>
    public double? Lon { get; set; }

    /// <summary>Latitude in degrees.</summary>
    public double? Lat { get; set; }

    /// <summary>Longitude, for data that came from a <c>[lat, lng]</c> world.</summary>
    public double? Lng { get; set; }

    /// <summary>Display name, used in tooltips and labels.</summary>
    public string? Name { get; set; }

    /// <summary>An optional value, for tooltips and labels. It does not size the marker.</summary>
    public double? Value { get; set; }

    /// <summary>Groups markers for colouring and the legend.</summary>
    public string? Category { get; set; }

    /// <summary>Shape for this marker, overriding the series'.</summary>
    public MarkerShape? Shape { get; set; }

    /// <summary>Fill for this marker, overriding the series'.</summary>
    public string? Color { get; set; }

    /// <summary>Size for this marker in pixels, overriding the series'.</summary>
    public double? Size { get; set; }

    /// <summary>Anything else on the row.</summary>
    [JsonExtensionData]
    public Dictionary<string, object?>? Extra { get; set; }
}

/// <summary>
/// An arc row. Both endpoints are required, because an arc without endpoints is not an arc. Each
/// is a position or a geometry key resolved against the current map.
/// </summary>
public class ArcDatum
{
    /// <summary>Where the route starts.</summary>
    public ArcEndpoint? From { get; set; }

    /// <summary>Where the route ends. Flow beads travel this way.</summary>
    public ArcEndpoint? To { get; set; }

    /// <summary>The value the route's width, or colour, carries.</summary>
    public double? Value { get; set; }

    /// <summary>Display name, used in tooltips.</summary>
    public string? Name { get; set; }

    /// <summary>Anything else on the row.</summary>
    [JsonExtensionData]
    public Dictionary<string, object?>? Extra { get; set; }
}

/// <summary>
/// One end of an arc: a position, or a geometry key (<c>"FRA"</c>) resolved to that feature's
/// centroid on the current map.
/// </summary>
[JsonConverter(typeof(ArcEndpointConverter))]
public sealed class ArcEndpoint
{
    /// <summary>An explicit position.</summary>
    public LonLat? Point { get; init; }

    /// <summary>A geometry key, resolved against the current map.</summary>
    public string? Key { get; init; }

    /// <summary>A position.</summary>
    public static implicit operator ArcEndpoint(LonLat point) => new() { Point = point };

    /// <summary>A <c>(lon, lat)</c> pair.</summary>
    public static implicit operator ArcEndpoint((double Lon, double Lat) point)
        => new() { Point = new LonLat(point.Lon, point.Lat) };

    /// <summary>A geometry key.</summary>
    public static implicit operator ArcEndpoint(string key) => new() { Key = key };
}

/// <summary>Writes an <see cref="ArcEndpoint"/> as <c>[lon, lat]</c> or as a key string.</summary>
public sealed class ArcEndpointConverter : JsonConverter<ArcEndpoint>
{
    /// <inheritdoc />
    public override ArcEndpoint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String
            ? new ArcEndpoint { Key = reader.GetString() }
            : new ArcEndpoint { Point = JsonSerializer.Deserialize<LonLat>(ref reader, options) };

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ArcEndpoint value, JsonSerializerOptions options)
    {
        if (value.Key is not null) writer.WriteStringValue(value.Key);
        else if (value.Point.HasValue) JsonSerializer.Serialize(writer, value.Point.Value, options);
        else writer.WriteNullValue();
    }
}

/// <summary>A route row: the vertices it passes through, in order.</summary>
public class LineDatum
{
    /// <summary>The vertex sequence, in order.</summary>
    public List<LonLat>? Path { get; set; }

    /// <summary>Stable identity, used to match a route across an update.</summary>
    public string? Id { get; set; }

    /// <summary>Display name, used in tooltips.</summary>
    public string? Name { get; set; }

    /// <summary>The value the route's width, or colour, carries.</summary>
    public double? Value { get; set; }

    /// <summary>Colour for this route, overriding the series'.</summary>
    public string? Color { get; set; }

    /// <summary>Anything else on the row.</summary>
    [JsonExtensionData]
    public Dictionary<string, object?>? Extra { get; set; }
}
