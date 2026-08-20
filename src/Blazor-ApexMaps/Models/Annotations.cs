using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor_ApexMaps.Models;

/// <summary>
/// The editorial overlay: the layer that says what the map is about.
/// </summary>
/// <remarks>
/// Annotations are deliberate statements, so they are never dropped for collision the way
/// generated labels are, and generated labels yield to them instead. They are also inert to the
/// pointer, so an annotation can sit over data without stealing its hover or click.
/// A licensed feature: works without a key, watermarked.
/// </remarks>
public class AnnotationOptions
{
    /// <summary>Annotations pinned to a coordinate.</summary>
    public List<PointAnnotation>? Points { get; set; }

    /// <summary>Annotations attached to a feature by key.</summary>
    public List<FeatureAnnotation>? Features { get; set; }

    /// <summary>Region highlights: a lon/lat box, or any geometry.</summary>
    public List<AreaAnnotation>? Areas { get; set; }
}

/// <summary>What every annotation has.</summary>
public abstract class AnnotationBase
{
    /// <summary>Stable identity, for update reconciliation and CSS targeting.</summary>
    public string? Id { get; set; }

    /// <summary>The text chip. A plain string is shorthand for its text.</summary>
    public AnnotationLabel? Label { get; set; }

    /// <summary>A symbol at the anchor, drawn with the marker shape set.</summary>
    public AnnotationMarker? Marker { get; set; }

    /// <summary>A leader line from the anchor to an offset label.</summary>
    public AnnotationConnector? Connector { get; set; }

    /// <summary>Extra class on the annotation's group.</summary>
    public string? ClassName { get; set; }
}

/// <summary>An annotation pinned to a coordinate.</summary>
public class PointAnnotation : AnnotationBase
{
    /// <summary>Where it sits.</summary>
    public LonLat At { get; set; }
}

/// <summary>
/// An annotation attached to a feature by key, anchored where the label engine would put it, so it
/// tracks the geometry through projection changes and lands where a reader would place it by hand.
/// </summary>
public class FeatureAnnotation : AnnotationBase
{
    /// <summary>The feature's join key.</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Trace the feature's own outline. <c>true</c> uses the theme's focus colour.</summary>
    public OutlineSpec? Outline { get; set; }
}

/// <summary>
/// A region highlight: a lon/lat box, or any GeoJSON geometry. Drawn in world space through the
/// projection, so it warps with the map rather than staying a screen-space rectangle over a curved
/// graticule.
/// </summary>
public class AreaAnnotation : AnnotationBase
{
    /// <summary>The box to highlight.</summary>
    public GeoBounds? Bounds { get; set; }

    /// <summary>Any GeoJSON geometry, for when a box is the wrong shape.</summary>
    public object? Geometry { get; set; }

    /// <summary>Fill colour.</summary>
    public string? Fill { get; set; }

    /// <summary>Fill opacity, 0 to 1.</summary>
    public double? FillOpacity { get; set; }

    /// <summary>Outline.</summary>
    public StrokeOptions? Stroke { get; set; }
}

/// <summary>
/// The text chip on an annotation.
/// </summary>
/// <remarks>
/// A chip rather than bare text is the default because an annotation has to stay readable over
/// whatever the map puts underneath it, and editorial text sitting directly on a choropleth is
/// legible on some classes and not others. <c>Background = "none"</c> gives haloed text with no
/// chip.
/// </remarks>
public class AnnotationLabel
{
    /// <summary>The text.</summary>
    public string? Text { get; set; }

    /// <summary>Which side of the anchor the chip sits on. Top by default.</summary>
    public AnnotationLabelPosition? Position { get; set; }

    /// <summary>Horizontal offset in pixels.</summary>
    public double? OffsetX { get; set; }

    /// <summary>Vertical offset in pixels.</summary>
    public double? OffsetY { get; set; }

    /// <summary>Text colour.</summary>
    public string? Color { get; set; }

    /// <summary>Font size in pixels.</summary>
    public double? FontSize { get; set; }

    /// <summary>Font weight, as a number or a CSS keyword.</summary>
    public object? FontWeight { get; set; }

    /// <summary>Chip fill. <c>"none"</c> draws haloed text with no chip.</summary>
    public string? Background { get; set; }

    /// <summary>Chip border colour.</summary>
    public string? BorderColor { get; set; }

    /// <summary>Chip border width in pixels.</summary>
    public double? BorderWidth { get; set; }

    /// <summary>Chip corner radius in pixels.</summary>
    public double? BorderRadius { get; set; }

    /// <summary>Chip padding in pixels.</summary>
    public double? Padding { get; set; }

    /// <summary>Text, with every other default.</summary>
    public static implicit operator AnnotationLabel(string text) => new() { Text = text };
}

/// <summary>A symbol at an annotation's anchor.</summary>
public class AnnotationMarker
{
    /// <summary>Draw it. Default true when a marker object is present.</summary>
    public bool? Show { get; set; }

    /// <summary>Which shape.</summary>
    public MarkerShape? Shape { get; set; }

    /// <summary>Size in pixels.</summary>
    public double? Size { get; set; }

    /// <summary>Fill colour.</summary>
    public string? Fill { get; set; }

    /// <summary>Outline.</summary>
    public StrokeOptions? Stroke { get; set; }
}

/// <summary>A leader line from an annotation's anchor to its offset label.</summary>
[JsonConverter(typeof(AnnotationConnectorConverter))]
public class AnnotationConnector
{
    /// <summary>Draw it.</summary>
    public bool Show { get; set; } = true;

    /// <summary>Line colour.</summary>
    public string? Color { get; set; }

    /// <summary>Line width in pixels.</summary>
    public double? Width { get; set; }

    /// <summary>SVG dash pattern.</summary>
    public string? DashArray { get; set; }

    /// <summary>Turn the connector on or off.</summary>
    public static implicit operator AnnotationConnector(bool show) => new() { Show = show };
}

/// <summary>
/// Writes an <see cref="AnnotationConnector"/> as a boolean when it carries no styling, and as its
/// options object otherwise: the core's option is <c>boolean | { color, width, dashArray }</c>,
/// with no "off" key on the object form.
/// </summary>
public sealed class AnnotationConnectorConverter : JsonConverter<AnnotationConnector>
{
    /// <inheritdoc />
    public override AnnotationConnector? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.True => new AnnotationConnector { Show = true },
            JsonTokenType.False => new AnnotationConnector { Show = false },
            _ => null,
        };

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, AnnotationConnector value, JsonSerializerOptions options)
    {
        if (!value.Show)
        {
            writer.WriteBooleanValue(false);
            return;
        }
        if (value.Color is null && value.Width is null && value.DashArray is null)
        {
            writer.WriteBooleanValue(true);
            return;
        }
        writer.WriteStartObject();
        if (value.Color is not null) writer.WriteString("color", value.Color);
        if (value.Width.HasValue) writer.WriteNumber("width", value.Width.Value);
        if (value.DashArray is not null) writer.WriteString("dashArray", value.DashArray);
        writer.WriteEndObject();
    }
}

/// <summary>
/// A feature outline: on with the theme's focus colour, off, or a stroke of your own.
/// </summary>
[JsonConverter(typeof(OutlineSpecConverter))]
public sealed class OutlineSpec
{
    /// <summary>Draw the outline.</summary>
    public bool Show { get; init; } = true;

    /// <summary>An explicit stroke, instead of the theme's focus colour.</summary>
    public StrokeOptions? Stroke { get; init; }

    /// <summary>On with the theme's focus colour, or off.</summary>
    public static implicit operator OutlineSpec(bool show) => new() { Show = show };

    /// <summary>An explicit stroke.</summary>
    public static implicit operator OutlineSpec(StrokeOptions stroke) => new() { Stroke = stroke };
}

/// <summary>Writes an <see cref="OutlineSpec"/> as a boolean or as stroke options.</summary>
public sealed class OutlineSpecConverter : JsonConverter<OutlineSpec>
{
    /// <inheritdoc />
    public override OutlineSpec? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.True => new OutlineSpec { Show = true },
            JsonTokenType.False => new OutlineSpec { Show = false },
            _ => null,
        };

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, OutlineSpec value, JsonSerializerOptions options)
    {
        if (value.Stroke is not null) JsonSerializer.Serialize(writer, value.Stroke, options);
        else writer.WriteBooleanValue(value.Show);
    }
}
