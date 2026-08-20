using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor_ApexMaps.Serialization;

namespace Blazor_ApexMaps.Models;

// The core's option types are TypeScript unions: `padding` is a number or an object,
// `palette` is a name or a colour list, `projection` is a name or a spec. C# has no
// untagged unions, so each one is a small sealed type with implicit conversions from the
// forms it accepts and a converter that writes whichever form was supplied. That keeps
// `Padding = 24` and `Padding = new Padding { Top = 8 }` both legal and both typed,
// instead of an `object?` that accepts anything and is checked by nobody.

/// <summary>
/// A geographic position, <c>[longitude, latitude]</c> in degrees (WGS84). Longitude first,
/// matching GeoJSON.
/// </summary>
[JsonConverter(typeof(LonLatConverter))]
public readonly struct LonLat
{
    /// <summary>Longitude in degrees, -180 to 180.</summary>
    public double Lon { get; }

    /// <summary>Latitude in degrees, -90 to 90.</summary>
    public double Lat { get; }

    /// <summary>Creates a position from longitude and latitude, in degrees.</summary>
    public LonLat(double lon, double lat)
    {
        Lon = lon;
        Lat = lat;
    }

    /// <summary>Creates a position from a <c>(lon, lat)</c> tuple.</summary>
    public static implicit operator LonLat((double Lon, double Lat) value) => new(value.Lon, value.Lat);

    /// <inheritdoc />
    public override string ToString() => $"[{Lon}, {Lat}]";
}

/// <summary>Writes a <see cref="LonLat"/> as <c>[lon, lat]</c>.</summary>
public sealed class LonLatConverter : JsonConverter<LonLat>
{
    /// <inheritdoc />
    public override LonLat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var values = JsonSerializer.Deserialize<double[]>(ref reader, options) ?? [];
        return values.Length >= 2 ? new LonLat(values[0], values[1]) : default;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, LonLat value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Lon);
        writer.WriteNumberValue(value.Lat);
        writer.WriteEndArray();
    }
}

/// <summary>
/// A geographic bounding box, <c>[west, south, east, north]</c> in degrees.
/// </summary>
[JsonConverter(typeof(GeoBoundsConverter))]
public readonly struct GeoBounds
{
    /// <summary>Western edge, in degrees of longitude.</summary>
    public double West { get; }

    /// <summary>Southern edge, in degrees of latitude.</summary>
    public double South { get; }

    /// <summary>Eastern edge, in degrees of longitude.</summary>
    public double East { get; }

    /// <summary>Northern edge, in degrees of latitude.</summary>
    public double North { get; }

    /// <summary>Creates a bounding box from its four edges, in degrees.</summary>
    public GeoBounds(double west, double south, double east, double north)
    {
        West = west;
        South = south;
        East = east;
        North = north;
    }

    /// <inheritdoc />
    public override string ToString() => $"[{West}, {South}, {East}, {North}]";
}

/// <summary>Writes a <see cref="GeoBounds"/> as <c>[west, south, east, north]</c>.</summary>
public sealed class GeoBoundsConverter : JsonConverter<GeoBounds>
{
    /// <inheritdoc />
    public override GeoBounds Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var v = JsonSerializer.Deserialize<double[]>(ref reader, options) ?? [];
        return v.Length >= 4 ? new GeoBounds(v[0], v[1], v[2], v[3]) : default;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, GeoBounds value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.West);
        writer.WriteNumberValue(value.South);
        writer.WriteNumberValue(value.East);
        writer.WriteNumberValue(value.North);
        writer.WriteEndArray();
    }
}

/// <summary>
/// Space between the plot's edge and its content: one value for every side, or a value per side.
/// </summary>
[JsonConverter(typeof(PaddingConverter))]
public sealed class Padding
{
    /// <summary>The same padding on every side, in pixels. Set by the implicit conversion.</summary>
    public double? All { get; set; }

    /// <summary>Top padding in pixels.</summary>
    public double? Top { get; set; }

    /// <summary>Right padding in pixels.</summary>
    public double? Right { get; set; }

    /// <summary>Bottom padding in pixels.</summary>
    public double? Bottom { get; set; }

    /// <summary>Left padding in pixels.</summary>
    public double? Left { get; set; }

    /// <summary>Uniform padding, in pixels.</summary>
    public static implicit operator Padding(double all) => new() { All = all };

    /// <summary>Uniform padding, in pixels.</summary>
    public static implicit operator Padding(int all) => new() { All = all };
}

/// <summary>Writes a <see cref="Padding"/> as a number or as a per-side object.</summary>
public sealed class PaddingConverter : JsonConverter<Padding>
{
    /// <inheritdoc />
    public override Padding? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number) return new Padding { All = reader.GetDouble() };
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        if (root.ValueKind != JsonValueKind.Object) return null;
        return new Padding
        {
            Top = Side(root, "top"),
            Right = Side(root, "right"),
            Bottom = Side(root, "bottom"),
            Left = Side(root, "left"),
        };

        static double? Side(JsonElement element, string name)
            => element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Number
                ? value.GetDouble()
                : null;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Padding value, JsonSerializerOptions options)
    {
        if (value.All.HasValue)
        {
            writer.WriteNumberValue(value.All.Value);
            return;
        }
        writer.WriteStartObject();
        if (value.Top.HasValue) writer.WriteNumber("top", value.Top.Value);
        if (value.Right.HasValue) writer.WriteNumber("right", value.Right.Value);
        if (value.Bottom.HasValue) writer.WriteNumber("bottom", value.Bottom.Value);
        if (value.Left.HasValue) writer.WriteNumber("left", value.Left.Value);
        writer.WriteEndObject();
    }
}

/// <summary>
/// A CSS length: a pixel count, or a string such as <c>"100%"</c> or <c>"32rem"</c>.
/// </summary>
[JsonConverter(typeof(DimensionConverter))]
public sealed class Dimension
{
    /// <summary>Pixel value, when the dimension was given as a number.</summary>
    public double? Pixels { get; init; }

    /// <summary>CSS length, when the dimension was given as a string.</summary>
    public string? Css { get; init; }

    /// <summary>A pixel count.</summary>
    public static implicit operator Dimension(double pixels) => new() { Pixels = pixels };

    /// <summary>A pixel count.</summary>
    public static implicit operator Dimension(int pixels) => new() { Pixels = pixels };

    /// <summary>A CSS length, such as <c>"100%"</c>.</summary>
    public static implicit operator Dimension(string css) => new() { Css = css };

    /// <inheritdoc />
    public override string ToString() => Css ?? Pixels?.ToString() ?? string.Empty;
}

/// <summary>Writes a <see cref="Dimension"/> as a number or a string.</summary>
public sealed class DimensionConverter : JsonConverter<Dimension>
{
    /// <inheritdoc />
    public override Dimension? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Number => new Dimension { Pixels = reader.GetDouble() },
            JsonTokenType.String => new Dimension { Css = reader.GetString() },
            _ => null,
        };

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Dimension value, JsonSerializerOptions options)
    {
        if (value.Css is not null) writer.WriteStringValue(value.Css);
        else if (value.Pixels.HasValue) writer.WriteNumberValue(value.Pixels.Value);
        else writer.WriteNullValue();
    }
}

/// <summary>
/// A switch with a third state: on, off, or <c>"auto"</c> (the core decides from context).
/// </summary>
[JsonConverter(typeof(AutoBoolConverter))]
public readonly struct AutoBool
{
    /// <summary>Whether the value is <c>"auto"</c>.</summary>
    public bool IsAuto { get; }

    /// <summary>The explicit value, when <see cref="IsAuto"/> is false.</summary>
    public bool Value { get; }

    private AutoBool(bool isAuto, bool value)
    {
        IsAuto = isAuto;
        Value = value;
    }

    /// <summary>Let the core decide from context.</summary>
    public static AutoBool Auto => new(true, false);

    /// <summary>On.</summary>
    public static AutoBool True => new(false, true);

    /// <summary>Off.</summary>
    public static AutoBool False => new(false, false);

    /// <summary>An explicit on or off.</summary>
    public static implicit operator AutoBool(bool value) => new(false, value);

    /// <inheritdoc />
    public override string ToString() => IsAuto ? "auto" : Value ? "true" : "false";
}

/// <summary>Writes an <see cref="AutoBool"/> as a boolean or the string <c>"auto"</c>.</summary>
public sealed class AutoBoolConverter : JsonConverter<AutoBool>
{
    /// <inheritdoc />
    public override AutoBool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.True => AutoBool.True,
            JsonTokenType.False => AutoBool.False,
            _ => AutoBool.Auto,
        };

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, AutoBool value, JsonSerializerOptions options)
    {
        if (value.IsAuto) writer.WriteStringValue("auto");
        else writer.WriteBooleanValue(value.Value);
    }
}

/// <summary>
/// A field on a data row: its name, or a JavaScript accessor that derives the value.
/// </summary>
[JsonConverter(typeof(FieldRefConverter))]
public sealed class FieldRef
{
    /// <summary>Property name on the data row.</summary>
    public string? Field { get; init; }

    /// <summary>Accessor called with the row, returning the value.</summary>
    public JsFunction? Accessor { get; init; }

    /// <summary>A property name on the data row.</summary>
    public static implicit operator FieldRef(string field) => new() { Field = field };

    /// <summary>An accessor called with the data row.</summary>
    public static implicit operator FieldRef(JsFunction accessor) => new() { Accessor = accessor };

    /// <inheritdoc />
    public override string ToString() => Field ?? Accessor?.Source ?? string.Empty;
}

/// <summary>Writes a <see cref="FieldRef"/> as a field name or as a function marker.</summary>
public sealed class FieldRefConverter : JsonConverter<FieldRef>
{
    /// <inheritdoc />
    public override FieldRef? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String ? new FieldRef { Field = reader.GetString() } : null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, FieldRef value, JsonSerializerOptions options)
    {
        if (value.Accessor is not null) JsonSerializer.Serialize(writer, value.Accessor, options);
        else if (value.Field is not null) writer.WriteStringValue(value.Field);
        else writer.WriteNullValue();
    }
}

/// <summary>
/// How a data row is matched to a geometry feature: one property name when both sides spell
/// it the same way, or one name per side.
/// </summary>
[JsonConverter(typeof(JoinSpecConverter))]
public sealed class JoinSpec
{
    /// <summary>Geometry property holding the key.</summary>
    public string? Geo { get; init; }

    /// <summary>Data property holding the key. Defaults to <see cref="Geo"/>.</summary>
    public string? Data { get; init; }

    /// <summary>One property name, used on both sides.</summary>
    public static implicit operator JoinSpec(string field) => new() { Geo = field, Data = field };

    /// <summary>A <c>(geometryProperty, dataProperty)</c> pair.</summary>
    public static implicit operator JoinSpec((string Geo, string Data) pair)
        => new() { Geo = pair.Geo, Data = pair.Data };

    /// <summary>A geometry property and a data property.</summary>
    public static JoinSpec Between(string geoField, string dataField)
        => new() { Geo = geoField, Data = dataField };
}

/// <summary>Writes a <see cref="JoinSpec"/> as a string, or as a <c>{ geo, data }</c> object.</summary>
public sealed class JoinSpecConverter : JsonConverter<JoinSpec>
{
    /// <inheritdoc />
    public override JoinSpec? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String
            ? new JoinSpec { Geo = reader.GetString(), Data = reader.GetString() }
            : null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, JoinSpec value, JsonSerializerOptions options)
    {
        if (value.Geo is not null && value.Data is not null && value.Geo == value.Data)
        {
            writer.WriteStringValue(value.Geo);
            return;
        }
        writer.WriteStartObject();
        if (value.Geo is not null) writer.WriteString("geo", value.Geo);
        if (value.Data is not null) writer.WriteString("data", value.Data);
        writer.WriteEndObject();
    }
}

/// <summary>
/// The colours a scale draws from: a registered palette name (see <see cref="Palettes"/>), or an
/// explicit colour list, which is taken literally rather than resampled.
/// </summary>
[JsonConverter(typeof(PaletteSpecConverter))]
public sealed class PaletteSpec
{
    /// <summary>Registered palette name.</summary>
    public string? Name { get; init; }

    /// <summary>Explicit colours, in order.</summary>
    public IReadOnlyList<string>? Colors { get; init; }

    /// <summary>A registered palette name.</summary>
    public static implicit operator PaletteSpec(string name) => new() { Name = name };

    /// <summary>An explicit colour list.</summary>
    public static implicit operator PaletteSpec(string[] colors) => new() { Colors = colors };

    /// <summary>An explicit colour list.</summary>
    public static implicit operator PaletteSpec(List<string> colors) => new() { Colors = colors };
}

/// <summary>Writes a <see cref="PaletteSpec"/> as a name or as a colour array.</summary>
public sealed class PaletteSpecConverter : JsonConverter<PaletteSpec>
{
    /// <inheritdoc />
    public override PaletteSpec? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String ? new PaletteSpec { Name = reader.GetString() } : null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, PaletteSpec value, JsonSerializerOptions options)
    {
        if (value.Colors is not null) JsonSerializer.Serialize(writer, value.Colors, options);
        else if (value.Name is not null) writer.WriteStringValue(value.Name);
        else writer.WriteNullValue();
    }
}

/// <summary>
/// Where geometry comes from: a registry id (<c>"world/countries@110m"</c>), a URL, or geometry
/// you already have in hand.
/// </summary>
/// <remarks>
/// Registry packs are fetched on demand, one request per pack however many maps ask for it, from
/// wherever <see cref="Configuration.ApexMapsConfiguration.GeoSource"/> points. Nothing is fetched
/// until a pack is named.
/// </remarks>
[JsonConverter(typeof(MapSourceConverter))]
public sealed class MapSource
{
    /// <summary>Registry id or URL.</summary>
    public string? Id { get; init; }

    /// <summary>
    /// Geometry as data: GeoJSON, TopoJSON or a bare geometry, as anything that serializes to the
    /// right JSON shape (a <see cref="Dictionary{TKey, TValue}"/>, a
    /// <see cref="System.Text.Json.JsonElement"/>, or your own model).
    /// </summary>
    public object? Geometry { get; init; }

    /// <summary>A registry id or a URL.</summary>
    public static implicit operator MapSource(string id) => new() { Id = id };

    /// <summary>Geometry passed directly, as GeoJSON or TopoJSON.</summary>
    public static MapSource FromGeometry(object geometry) => new() { Geometry = geometry };

    /// <inheritdoc />
    public override string ToString() => Id ?? "(inline geometry)";
}

/// <summary>Writes a <see cref="MapSource"/> as an id string or as inline geometry.</summary>
public sealed class MapSourceConverter : JsonConverter<MapSource>
{
    /// <inheritdoc />
    public override MapSource? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String ? new MapSource { Id = reader.GetString() } : null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, MapSource value, JsonSerializerOptions options)
    {
        if (value.Geometry is not null) JsonSerializer.Serialize(writer, value.Geometry, options);
        else if (value.Id is not null) writer.WriteStringValue(value.Id);
        else writer.WriteNullValue();
    }
}

/// <summary>
/// What the opening view frames: the data, the whole world, nothing (leave the camera where the
/// projection put it), or an explicit box.
/// </summary>
[JsonConverter(typeof(ViewFitConverter))]
public sealed class ViewFit
{
    /// <summary>Named fit mode.</summary>
    public ViewFitMode? Mode { get; init; }

    /// <summary>Explicit box to frame.</summary>
    public GeoBounds? Bounds { get; init; }

    /// <summary>Frame the geometry the series cover.</summary>
    public static ViewFit Data => new() { Mode = ViewFitMode.Data };

    /// <summary>Frame the whole world.</summary>
    public static ViewFit World => new() { Mode = ViewFitMode.World };

    /// <summary>Leave the camera where the projection put it.</summary>
    public static ViewFit None => new() { Mode = ViewFitMode.None };

    /// <summary>A named fit mode.</summary>
    public static implicit operator ViewFit(ViewFitMode mode) => new() { Mode = mode };

    /// <summary>An explicit box, in degrees.</summary>
    public static implicit operator ViewFit(GeoBounds bounds) => new() { Bounds = bounds };
}

/// <summary>Writes a <see cref="ViewFit"/> as a mode name or as a bounding box.</summary>
public sealed class ViewFitConverter : JsonConverter<ViewFit>
{
    /// <inheritdoc />
    public override ViewFit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ViewFit value, JsonSerializerOptions options)
    {
        if (value.Bounds.HasValue) JsonSerializer.Serialize(writer, value.Bounds.Value, options);
        else if (value.Mode.HasValue) JsonSerializer.Serialize(writer, value.Mode.Value, options);
        else writer.WriteNullValue();
    }
}

/// <summary>
/// How fast value transitions run: a preset, or an explicit duration in milliseconds.
/// </summary>
[JsonConverter(typeof(AnimationSpeedConverter))]
public sealed class AnimationSpeed
{
    /// <summary>Preset speed.</summary>
    public AnimationSpeedPreset? Preset { get; init; }

    /// <summary>Duration in milliseconds.</summary>
    public double? Milliseconds { get; init; }

    /// <summary>A preset speed.</summary>
    public static implicit operator AnimationSpeed(AnimationSpeedPreset preset) => new() { Preset = preset };

    /// <summary>A duration in milliseconds.</summary>
    public static implicit operator AnimationSpeed(double milliseconds) => new() { Milliseconds = milliseconds };

    /// <summary>A duration in milliseconds.</summary>
    public static implicit operator AnimationSpeed(int milliseconds) => new() { Milliseconds = milliseconds };
}

/// <summary>Writes an <see cref="AnimationSpeed"/> as a preset name or a number.</summary>
public sealed class AnimationSpeedConverter : JsonConverter<AnimationSpeed>
{
    /// <inheritdoc />
    public override AnimationSpeed? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Number
            ? new AnimationSpeed { Milliseconds = reader.GetDouble() }
            : null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, AnimationSpeed value, JsonSerializerOptions options)
    {
        if (value.Milliseconds.HasValue) writer.WriteNumberValue(value.Milliseconds.Value);
        else if (value.Preset.HasValue) JsonSerializer.Serialize(writer, value.Preset.Value, options);
        else writer.WriteNullValue();
    }
}

/// <summary>
/// A pattern fill: one tile for the whole series, or a decision made per feature.
/// </summary>
[JsonConverter(typeof(PatternSpecConverter))]
public sealed class PatternSpec
{
    /// <summary>One pattern, for every feature in the series.</summary>
    public PatternFillOptions? Pattern { get; init; }

    /// <summary>
    /// A pattern decided per feature: called with the feature's context, returning pattern options
    /// or null to leave that feature flat.
    /// </summary>
    public JsFunction? Decide { get; init; }

    /// <summary>One pattern for the series.</summary>
    public static implicit operator PatternSpec(PatternFillOptions pattern) => new() { Pattern = pattern };

    /// <summary>A pattern decided per feature.</summary>
    public static implicit operator PatternSpec(JsFunction decide) => new() { Decide = decide };
}

/// <summary>Writes a <see cref="PatternSpec"/> as pattern options or as a function marker.</summary>
public sealed class PatternSpecConverter : JsonConverter<PatternSpec>
{
    /// <inheritdoc />
    public override PatternSpec? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, PatternSpec value, JsonSerializerOptions options)
    {
        if (value.Decide is not null) JsonSerializer.Serialize(writer, value.Decide, options);
        else if (value.Pattern is not null) JsonSerializer.Serialize(writer, value.Pattern, options);
        else writer.WriteNullValue();
    }
}

/// <summary>
/// An image fill's source: one URL, or a URL chosen per feature.
/// </summary>
[JsonConverter(typeof(ImageSourceConverter))]
public sealed class ImageSource
{
    /// <summary>One URL, for every feature in the series.</summary>
    public string? Url { get; init; }

    /// <summary>
    /// A URL chosen per feature: called with the feature's context, returning a URL or null.
    /// </summary>
    public JsFunction? Decide { get; init; }

    /// <summary>One URL for every feature.</summary>
    public static implicit operator ImageSource(string url) => new() { Url = url };

    /// <summary>A URL chosen per feature.</summary>
    public static implicit operator ImageSource(JsFunction decide) => new() { Decide = decide };
}

/// <summary>Writes an <see cref="ImageSource"/> as a URL or as a function marker.</summary>
public sealed class ImageSourceConverter : JsonConverter<ImageSource>
{
    /// <inheritdoc />
    public override ImageSource? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String ? new ImageSource { Url = reader.GetString() } : null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ImageSource value, JsonSerializerOptions options)
    {
        if (value.Decide is not null) JsonSerializer.Serialize(writer, value.Decide, options);
        else if (value.Url is not null) writer.WriteStringValue(value.Url);
        else writer.WriteNullValue();
    }
}

/// <summary>
/// A drilldown's child map: a fixed source, or one resolved per clicked feature.
/// </summary>
[JsonConverter(typeof(DrilldownTargetConverter))]
public sealed class DrilldownTarget
{
    /// <summary>The same child map for every feature.</summary>
    public MapSource? Map { get; init; }

    /// <summary>
    /// The child map resolved per feature: called with the clicked feature's context, returning a
    /// registry id, a URL, geometry, or null to refuse the drill.
    /// </summary>
    public JsFunction? Resolve { get; init; }

    /// <summary>A registry id or a URL.</summary>
    public static implicit operator DrilldownTarget(string id) => new() { Map = id };

    /// <summary>A fixed map source.</summary>
    public static implicit operator DrilldownTarget(MapSource map) => new() { Map = map };

    /// <summary>A map resolved per clicked feature.</summary>
    public static implicit operator DrilldownTarget(JsFunction resolve) => new() { Resolve = resolve };
}

/// <summary>Writes a <see cref="DrilldownTarget"/> as a map source or as a function marker.</summary>
public sealed class DrilldownTargetConverter : JsonConverter<DrilldownTarget>
{
    /// <inheritdoc />
    public override DrilldownTarget? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String ? new DrilldownTarget { Map = reader.GetString()! } : null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DrilldownTarget value, JsonSerializerOptions options)
    {
        if (value.Resolve is not null) JsonSerializer.Serialize(writer, value.Resolve, options);
        else if (value.Map is not null) JsonSerializer.Serialize(writer, value.Map, options);
        else writer.WriteNullValue();
    }
}

/// <summary>
/// A marker's shape: one shape for the series, or a shape chosen per row.
/// </summary>
[JsonConverter(typeof(MarkerShapeRefConverter))]
public sealed class MarkerShapeRef
{
    /// <summary>One shape, for every marker in the series.</summary>
    public MarkerShape? Shape { get; init; }

    /// <summary>A shape chosen per row: called with the row, returning a shape name.</summary>
    public JsFunction? Decide { get; init; }

    /// <summary>One shape for the series.</summary>
    public static implicit operator MarkerShapeRef(MarkerShape shape) => new() { Shape = shape };

    /// <summary>A shape chosen per row.</summary>
    public static implicit operator MarkerShapeRef(JsFunction decide) => new() { Decide = decide };
}

/// <summary>Writes a <see cref="MarkerShapeRef"/> as a shape name or as a function marker.</summary>
public sealed class MarkerShapeRefConverter : JsonConverter<MarkerShapeRef>
{
    /// <inheritdoc />
    public override MarkerShapeRef? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, MarkerShapeRef value, JsonSerializerOptions options)
    {
        if (value.Decide is not null) JsonSerializer.Serialize(writer, value.Decide, options);
        else if (value.Shape.HasValue) JsonSerializer.Serialize(writer, value.Shape.Value, options);
        else writer.WriteNullValue();
    }
}
