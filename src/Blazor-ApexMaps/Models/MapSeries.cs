using System.Text.Json.Serialization;
using Blazor_ApexMaps.Serialization;

namespace Blazor_ApexMaps.Models;

/// <summary>
/// What a series has whatever its type. Use one of the five concrete types:
/// <see cref="ChoroplethSeries"/>, <see cref="BubbleSeries"/>, <see cref="MarkerSeries"/>,
/// <see cref="ArcSeries"/> or <see cref="LineSeries"/>.
/// </summary>
/// <remarks>
/// The core's <c>Series</c> is a discriminated union, so the <c>type</c> key decides which other
/// keys are legal: an arc needs endpoints, a choropleth needs a join. Here that is one class per
/// type, and the <c>type</c> key is written by the serializer from the class you chose, so it can
/// never disagree with the options you set.
/// </remarks>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ChoroplethSeries), "choropleth")]
[JsonDerivedType(typeof(BubbleSeries), "bubble")]
[JsonDerivedType(typeof(MarkerSeries), "marker")]
[JsonDerivedType(typeof(ArcSeries), "arc")]
[JsonDerivedType(typeof(LineSeries), "line")]
public abstract class MapSeries
{
    /// <summary>
    /// Series name. Used as the legend title, in tooltips, and to match a series across an update.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>Draw it. Default true.</summary>
    public bool? Visible { get; set; }

    /// <summary>Series opacity, 0 to 1.</summary>
    public double? Opacity { get; set; }

    /// <summary>Outline for the series' marks.</summary>
    public StrokeOptions? Stroke { get; set; }

    /// <summary>Per-mark labels drawn from the series' own rows.</summary>
    public SeriesLabelOptions? Labels { get; set; }

    /// <summary>How the series' marks enter.</summary>
    public MarkAnimationOptions? Animation { get; set; }

    /// <summary>Field or accessor for the primary value. Default <c>"value"</c>.</summary>
    public FieldRef? ValueField { get; set; }

    /// <summary>
    /// Anything this wrapper does not model, merged into the series. Keys are written as given, so
    /// they must be spelled the way the core spells them.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object?>? AdditionalOptions { get; set; }
}

/// <summary>Per-mark labels drawn from the series' own rows.</summary>
public class SeriesLabelOptions
{
    /// <summary>Draw them. Default false.</summary>
    public bool? Show { get; set; }

    /// <summary>Field or accessor supplying the text.</summary>
    public FieldRef? Field { get; set; }
}

/// <summary>How a series' marks enter.</summary>
public class MarkAnimationOptions
{
    /// <summary>Grow for symbols, draw for paths, fade for either.</summary>
    public MarkAnimationType? Type { get; set; }

    /// <summary>Duration in milliseconds.</summary>
    public double? Duration { get; set; }

    /// <summary>Per-mark delay in milliseconds, applied in render order.</summary>
    public double? Stagger { get; set; }

    /// <summary>Easing name.</summary>
    public string? Ease { get; set; }
}

// --- choropleth --------------------------------------------------------------

/// <summary>
/// Areas coloured by value: the map everyone means by "a data map". Rows are matched to features
/// by <see cref="JoinBy"/>, and features with no row keep the no-data colour rather than falling
/// into the lowest class.
/// </summary>
public class ChoroplethSeries : MapSeries
{
    /// <summary>
    /// How rows match features: one property name, or one per side. Detected when omitted, and
    /// registry packs carry a recommended geometry key.
    /// </summary>
    public JoinSpec? JoinBy { get; set; }

    /// <summary>
    /// The rows. Anything that serializes to objects: anonymous types, records, dictionaries.
    /// Property names are camelCased on the way out, so a row's join key is <c>"code"</c> for a
    /// <c>Code</c> property.
    /// </summary>
    public IEnumerable<object>? Data { get; set; }

    /// <summary>
    /// Apply normalised and alias matches, reporting each substitution. Off by default, because
    /// silently guessing turns an obvious failure into a plausible wrong answer.
    /// </summary>
    public bool? FuzzyJoin { get; set; }

    /// <summary>How values become colours.</summary>
    public ScaleOptions? Scale { get; set; }

    /// <summary>
    /// Texture instead of, or over, the flat fill. A licensed feature: works without a key,
    /// watermarked.
    /// </summary>
    public SeriesFillOptions? Fill { get; set; }

    /// <summary>
    /// Divide the value by another field before mapping, such as <c>"population"</c>. The legend
    /// retitles itself, because a choropleth of counts across unequal areas mostly redraws the
    /// population map.
    /// </summary>
    public string? NormalizeBy { get; set; }

    /// <summary>
    /// Click a feature to replace the map with a deeper level. A licensed feature: works without
    /// a key, watermarked.
    /// </summary>
    public DrilldownOptions? Drilldown { get; set; }
}

/// <summary>Texture instead of, or over, a flat fill.</summary>
/// <remarks>
/// Image wins where both are set. No-data features are never textured: an absence has to keep
/// reading as an absence, and a pattern over it reads as one more category.
/// </remarks>
public class SeriesFillOptions
{
    /// <summary>
    /// One pattern for the series, or a <see cref="JsFunction"/> deciding per feature. The
    /// function is called with the feature's context, including the colour the scale already
    /// resolved, and may return null to leave that feature flat.
    /// </summary>
    public PatternSpec? Pattern { get; set; }

    /// <summary>A picture per region, clipped to its own outline.</summary>
    public ImageFillOptions? Image { get; set; }
}

/// <summary>A pattern tile drawn over a feature's fill.</summary>
public class PatternFillOptions
{
    /// <summary>Which tile. Dots by default.</summary>
    public PatternType? Type { get; set; }

    /// <summary>Tile geometry for a custom type, drawn in a <c>size</c> by <c>size</c> box.</summary>
    public string? Path { get; set; }

    /// <summary>
    /// Spacing between one mark and the next, in screen pixels. Default 10, and screen pixels
    /// rather than world, so the texture holds its size as the reader zooms. Tightening it is how
    /// a patterned map turns muddy: the ink averages with the colour into a shade on no scale.
    /// </summary>
    public double? Size { get; set; }

    /// <summary>
    /// Ink colour. Defaults to white on a background dark enough to carry it, and to a darkened
    /// tint otherwise, so the texture stays legible across a whole ramp.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>Tile background. Defaults to the colour the scale resolved for the feature.</summary>
    public string? Background { get; set; }

    /// <summary>Ink weight for line tiles. Default a fifth of the size, so gaps dominate.</summary>
    public double? StrokeWidth { get; set; }

    /// <summary>Rotate the tile, in degrees.</summary>
    public double? Angle { get; set; }

    /// <summary>Ink opacity, 0 to 1.</summary>
    public double? Opacity { get; set; }
}

/// <summary>A picture per region, clipped to its own outline.</summary>
public class ImageFillOptions
{
    /// <summary>
    /// The image: one URL for every feature, or a <see cref="JsFunction"/> called per feature and
    /// returning a URL (or null, to leave that feature on its flat colour). The function form is
    /// the useful one, and how each region gets its own flag or portrait.
    /// </summary>
    public ImageSource? Src { get; set; }

    /// <summary>How the image sits in the feature's bounding box. Cover by default.</summary>
    public ImageFit? Fit { get; set; }

    /// <summary>
    /// Painted under the image. Defaults to the feature's flat colour, which is what shows through
    /// a contained fit and while the image is loading.
    /// </summary>
    public string? Background { get; set; }

    /// <summary>Image opacity, 0 to 1.</summary>
    public double? Opacity { get; set; }
}

/// <summary>Click a feature to replace the map with a deeper level.</summary>
/// <remarks>
/// A series keeps its own data across levels, so one array holding rows for both levels (states
/// and counties, keyed the same way) needs no extra wiring. For data fetched per level, handle
/// <c>OnDrilldown</c> and update the series from there: the child level is already on screen by
/// then.
/// </remarks>
public class DrilldownOptions
{
    /// <summary>
    /// The child map: a registry id, a URL, geometry, or a <see cref="JsFunction"/> called with
    /// the clicked feature's context and returning one of those (or null to refuse, which is how a
    /// map with children for only some features declines the rest).
    /// </summary>
    public DrilldownTarget? Map { get; set; }

    /// <summary>
    /// Which child features belong to the clicked parent. Auto by default, which looks for a child
    /// property holding the parent's key and then falls back to a key prefix, and is enough for
    /// nearly all published hierarchical geometry.
    /// </summary>
    public DrilldownScope? Scope { get; set; }

    /// <summary>Child property holding the parent's key. Skips detection.</summary>
    public string? ParentField { get; set; }

    /// <summary>
    /// How the swap looks. Zoom by default: the geography never moves across the swap, and the
    /// child develops out of the parent's own colour, so what the reader sees is the parent shape
    /// dividing into its children.
    /// </summary>
    public DrilldownAnimation? Animate { get; set; }

    /// <summary>The trail above the map, with a way back up. Default true.</summary>
    public BreadcrumbOptions? Breadcrumb { get; set; }
}

/// <summary>The drilldown trail above the map.</summary>
[JsonConverter(typeof(BreadcrumbOptionsConverter))]
public class BreadcrumbOptions
{
    /// <summary>Show it. Default true.</summary>
    public bool Show { get; set; } = true;

    /// <summary>Label for the top level. Default "All".</summary>
    public string? RootLabel { get; set; }

    /// <summary>Turn the breadcrumb on or off.</summary>
    public static implicit operator BreadcrumbOptions(bool show) => new() { Show = show };
}

/// <summary>
/// Writes a <see cref="BreadcrumbOptions"/> as <c>false</c> when it is off, and as its options
/// object otherwise. The core's own option is <c>boolean | { rootLabel }</c>, with no "off" key on
/// the object form, so "off" has to leave as a boolean.
/// </summary>
public sealed class BreadcrumbOptionsConverter : System.Text.Json.Serialization.JsonConverter<BreadcrumbOptions>
{
    /// <inheritdoc />
    public override BreadcrumbOptions? Read(
        ref System.Text.Json.Utf8JsonReader reader,
        Type typeToConvert,
        System.Text.Json.JsonSerializerOptions options)
        => reader.TokenType == System.Text.Json.JsonTokenType.False ? new BreadcrumbOptions { Show = false } : null;

    /// <inheritdoc />
    public override void Write(
        System.Text.Json.Utf8JsonWriter writer,
        BreadcrumbOptions value,
        System.Text.Json.JsonSerializerOptions options)
    {
        if (!value.Show)
        {
            writer.WriteBooleanValue(false);
            return;
        }
        writer.WriteStartObject();
        if (value.RootLabel is not null) writer.WriteString("rootLabel", value.RootLabel);
        writer.WriteEndObject();
    }
}

// --- bubble ------------------------------------------------------------------

/// <summary>
/// Proportional symbols: a circle per row, its area carrying the value. The right series when a
/// quantity belongs to a place rather than to an area, or when the areas are too unequal for a
/// choropleth to be read.
/// </summary>
public class BubbleSeries : MapSeries
{
    /// <summary>The rows: a position, or a join key resolved to a feature's centroid.</summary>
    public IEnumerable<BubbleDatum>? Data { get; set; }

    /// <summary>Join to geometry centroids when the rows carry no coordinates.</summary>
    public JoinSpec? JoinBy { get; set; }

    /// <summary>Apply normalised and alias matches when joining.</summary>
    public bool? FuzzyJoin { get; set; }

    /// <summary>How the value becomes a radius. Square root by default, and correctly so.</summary>
    public SizeOptions? Size { get; set; }

    /// <summary>One fill for every bubble.</summary>
    public string? Color { get; set; }

    /// <summary>A colour scale, to encode a second variable in the fill.</summary>
    public ScaleOptions? ColorScale { get; set; }

    /// <summary>Field or accessor for the colour value, when a colour scale is set.</summary>
    public FieldRef? ColorField { get; set; }

    /// <summary>
    /// Draw largest first, so small bubbles stay clickable on top. Default true, and turning it
    /// off is almost always a mistake.
    /// </summary>
    public bool? SortBySize { get; set; }
}

// --- marker ------------------------------------------------------------------

/// <summary>
/// Points of interest at a fixed size: seven generated shapes, optional categorical colour, and
/// optional clustering. A marker says "something is here"; when size should carry a quantity,
/// that is <see cref="BubbleSeries"/>.
/// </summary>
public class MarkerSeries : MapSeries
{
    /// <summary>The rows: a position, or a join key resolved to a feature's centroid.</summary>
    public IEnumerable<MarkerDatum>? Data { get; set; }

    /// <summary>Join to geometry centroids when the rows carry no coordinates.</summary>
    public JoinSpec? JoinBy { get; set; }

    /// <summary>Apply normalised and alias matches when joining.</summary>
    public bool? FuzzyJoin { get; set; }

    /// <summary>
    /// Shape for every marker (circle by default), or a <see cref="JsFunction"/> called with the
    /// row and returning a shape name.
    /// </summary>
    public MarkerShapeRef? Shape { get; set; }

    /// <summary>Width of the shape's bounding box, in pixels. Default 10.</summary>
    public double? Size { get; set; }

    /// <summary>One fill for every marker.</summary>
    public string? Color { get; set; }

    /// <summary>Row field holding a category, for categorical colour and a legend.</summary>
    public string? ColorBy { get; set; }

    /// <summary>Palette the categories are drawn from.</summary>
    public string? Palette { get; set; }

    /// <summary>
    /// Merge markers that would pile up, dissolving as the reader zooms in. A licensed feature:
    /// works without a key, watermarked.
    /// </summary>
    public ClusterOptions? Cluster { get; set; }
}

/// <summary>
/// How markers that would pile up are merged.
/// </summary>
/// <remarks>
/// Clustering is an option on the marker series rather than a series of its own: the data is
/// identical either way, and it is a decision about how to draw points, in the same way that a
/// choropleth's classification is a decision about how to colour values.
/// </remarks>
public class ClusterOptions
{
    /// <summary>Enable it. Default true whenever a cluster object is present at all.</summary>
    public bool? Enabled { get; set; }

    /// <summary>Screen-space merge distance in pixels. Default 60.</summary>
    public double? Radius { get; set; }

    /// <summary>Above this zoom, draw individual markers. Default 8.</summary>
    public double? MaxZoom { get; set; }

    /// <summary>Fewer members than this stay as individual markers. Default 2.</summary>
    public int? MinPoints { get; set; }

    /// <summary>Circle radius range in pixels, smallest count to largest.</summary>
    public double[]? Size { get; set; }

    /// <summary>Circle fill.</summary>
    public string? Color { get; set; }

    /// <summary>Show the member count inside the circle. Default true.</summary>
    public bool? ShowCount { get; set; }

    /// <summary>Zoom to the members' bounds when a cluster is clicked. Default true.</summary>
    public bool? ZoomOnClick { get; set; }
}

// --- arc ---------------------------------------------------------------------

/// <summary>
/// Great-circle connections between two endpoints: flights, trade, migration, traffic.
/// A licensed feature: works without a key, watermarked.
/// </summary>
public class ArcSeries : MapSeries
{
    /// <summary>The routes. Each needs a start and an end.</summary>
    public IEnumerable<ArcDatum>? Data { get; set; }

    /// <summary>
    /// Follow the great circle. Default true, and it matters: a straight line between Tokyo and
    /// New York in Mercator is not the route a plane flies, and naive straight lines break at the
    /// antimeridian.
    /// </summary>
    public bool? Geodesic { get; set; }

    /// <summary>
    /// Bulge the arc perpendicular to its chord, 0 to about 1. Decorative rather than geographic:
    /// a curved arc is no longer the true path, so it is off by default.
    /// </summary>
    public double? Curvature { get; set; }

    /// <summary>Line width scale driven by the value.</summary>
    public SizeOptions? Width { get; set; }

    /// <summary>One colour for every route.</summary>
    public string? Color { get; set; }

    /// <summary>A colour scale, to encode the value in the stroke.</summary>
    public ScaleOptions? ColorScale { get; set; }

    /// <summary>Draw a dot at each end.</summary>
    public EndpointOptions? Endpoints { get; set; }

    /// <summary>
    /// Send beads along the route, from its start towards its end, so a connection reads as a
    /// direction. An arc says two places are related; it does not say which way anything moves.
    /// </summary>
    public FlowOptions? Flow { get; set; }

    /// <summary>Resolve endpoints given as strings against geometry keys.</summary>
    public JoinSpec? JoinBy { get; set; }
}

/// <summary>
/// A route drawn through the vertices it is given, in order: a GPS trace, a shipping lane, a
/// transit line. Unlike an arc, which derives the great circle between two endpoints, the caller
/// supplies the whole path. A licensed feature: works without a key, watermarked.
/// </summary>
public class LineSeries : MapSeries
{
    /// <summary>The routes, each with its vertex sequence.</summary>
    public IEnumerable<LineDatum>? Data { get; set; }

    /// <summary>Line width scale driven by the value.</summary>
    public SizeOptions? Width { get; set; }

    /// <summary>One colour for every route.</summary>
    public string? Color { get; set; }

    /// <summary>A colour scale, to encode the value in the stroke.</summary>
    public ScaleOptions? ColorScale { get; set; }

    /// <summary>Draw a dot at each route's start and end.</summary>
    public EndpointOptions? Endpoints { get; set; }

    /// <summary>Send beads along the route, in the order its vertices were given.</summary>
    public FlowOptions? Flow { get; set; }
}

/// <summary>Dots at a route's ends.</summary>
public class EndpointOptions
{
    /// <summary>Draw them. Default false.</summary>
    public bool? Show { get; set; }

    /// <summary>Dot radius in pixels.</summary>
    public double? Radius { get; set; }

    /// <summary>Dot fill. Defaults to the route's colour.</summary>
    public string? Color { get; set; }
}

/// <summary>
/// Beads travelling along a route, so a connection reads as a direction.
/// </summary>
/// <remarks>
/// Drawn as a dashed companion path animated on its dash offset, which is why speed and spacing
/// are in screen pixels and the route's length never enters into it. Reduced motion, animations
/// off, and a route count past the flow budget all leave the beads in place and stop them moving,
/// because a dotted route still reads as a route.
/// </remarks>
public class FlowOptions
{
    /// <summary>Beads, or a marching dashed highlight. Beads by default.</summary>
    public FlowStyle? Style { get; set; }

    /// <summary>
    /// Whether the beads belong to the ground or to the screen. Zoom by default, so a bead stays
    /// over the same stretch of route as the reader zooms; each of size, pace and spacing is
    /// bounded, because each degenerates at the far end of the camera.
    /// </summary>
    public FlowScale? Scale { get; set; }

    /// <summary>
    /// Travel speed in screen pixels per second, at the zoom the map opened at. Default 90.
    /// </summary>
    public double? Speed { get; set; }

    /// <summary>
    /// Screen pixels between one bead and the next, at the opening zoom. Default 56, so an
    /// ordinary route carries a handful of beads rather than a dotted texture. Lower it for a map
    /// of short routes.
    /// </summary>
    public double? Spacing { get; set; }

    /// <summary>Bead diameter, or dash weight, in pixels. Defaults to the route's own width.</summary>
    public double? Size { get; set; }

    /// <summary>Bead colour. Defaults to the route's colour.</summary>
    public string? Color { get; set; }

    /// <summary>Bead opacity, 0 to 1. Default 1.</summary>
    public double? Opacity { get; set; }

    /// <summary>
    /// Offset each route's phase, so a corridor of parallel routes reads as traffic rather than
    /// one synchronised pulse. Default true.
    /// </summary>
    public bool? Stagger { get; set; }

    /// <summary>
    /// Flow with every default, so <c>Flow = true</c> reads as it does in JavaScript. Off is the
    /// absence of the option, so <c>Flow = false</c> is null.
    /// </summary>
    public static implicit operator FlowOptions?(bool enabled) => enabled ? new FlowOptions() : null;
}
