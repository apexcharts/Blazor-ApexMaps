using System.Text.Json.Serialization;
using Blazor_ApexMaps.Serialization;

namespace Blazor_ApexMaps.Models;

/// <summary>
/// The whole option tree for a map. Every branch is optional: a map needs a geometry source and,
/// usually, one series.
/// </summary>
/// <remarks>
/// <example>
/// <code>
/// var options = new MapsOptions
/// {
///     Geo = new GeoOptions { Map = GeoMaps.WorldCountries110m },
///     Series =
///     [
///         new ChoroplethSeries
///         {
///             Name = "Unemployment rate",
///             JoinBy = JoinSpec.Between("iso_a3", "code"),
///             Data = [new { code = "FRA", value = 7.3 }, new { code = "DEU", value = 5.7 }],
///         },
///     ],
/// };
/// </code>
/// </example>
/// </remarks>
public class MapsOptions
{
    /// <summary>Size, theme context and animation: what the chart is, rather than what it shows.</summary>
    public ChartOptions? Chart { get; set; }

    /// <summary>Geometry, projection and opening view.</summary>
    public GeoOptions? Geo { get; set; }

    /// <summary>
    /// What is drawn on the geometry. Mix types freely: a choropleth under bubbles under arcs is
    /// one map with three series. With no series at all the geometry draws as a plain basemap.
    /// </summary>
    public List<MapSeries>? Series { get; set; }

    /// <summary>The scale, printed.</summary>
    public LegendOptions? Legend { get; set; }

    /// <summary>What hovering a mark says.</summary>
    public TooltipOptions? Tooltip { get; set; }

    /// <summary>Values written onto the map itself.</summary>
    public DataLabelOptions? DataLabels { get; set; }

    /// <summary>How hover, selection and muting look.</summary>
    public StatesOptions? States { get; set; }

    /// <summary>Light or dark chrome, and the categorical palette series draw from.</summary>
    public ThemeOptions? Theme { get; set; }

    /// <summary>Zoom, pan, globe rotation, selection and hit assistance.</summary>
    public InteractionOptions? Interaction { get; set; }

    /// <summary>
    /// The accessibility layer: ARIA roles, a generated description, keyboard navigation and an
    /// optional data table. On by default, and free in every tier.
    /// </summary>
    public A11yOptions? A11y { get; set; }

    /// <summary>
    /// The editorial layer: the annotations that say what the map is about.
    /// A licensed feature: works without a key, watermarked.
    /// </summary>
    public AnnotationOptions? Annotations { get; set; }

    /// <summary>
    /// Cross-filter group. Maps naming the same group share their selection, so brushing one
    /// brushes the others. A licensed feature: works without a key, watermarked.
    /// </summary>
    public LinkOptions? Link { get; set; }

    /// <summary>Development diagnostics, including the join report.</summary>
    public DebugOptions? Debug { get; set; }

    /// <summary>
    /// Option overrides applied below a container width, narrowest rule last. A rule's options are
    /// merged over the base, so a narrow layout can drop the legend and thin the strokes without
    /// restating the map.
    /// </summary>
    public List<ResponsiveRule>? Responsive { get; set; }

    /// <summary>
    /// Anything the models above do not cover, merged into the options at the top level.
    /// </summary>
    /// <remarks>
    /// The escape hatch for a core option newer than this wrapper. Keys are written exactly as
    /// given, so they must be spelled the way the core spells them (camelCase). Function-valued
    /// options go through <see cref="JsFunction"/>.
    /// </remarks>
    [JsonExtensionData]
    public Dictionary<string, object?>? AdditionalOptions { get; set; }
}

/// <summary>Size, theme context and animation.</summary>
public class ChartOptions
{
    /// <summary>Plot width: pixels, or a CSS length such as <c>"100%"</c>.</summary>
    public Dimension? Width { get; set; }

    /// <summary>Plot height: pixels, or a CSS length. Defaults to 400.</summary>
    public Dimension? Height { get; set; }

    /// <summary>Seeds the default type for series that do not state one.</summary>
    public SeriesType? Type { get; set; }

    /// <summary>
    /// Painted behind the map. Transparent by default, so the card underneath shows through.
    /// </summary>
    public string? Background { get; set; }

    /// <summary>Font stack for the map's own text.</summary>
    public string? FontFamily { get; set; }

    /// <summary>
    /// What the chart is for. <c>Story</c> animates entrances; <c>Dashboard</c>, the default,
    /// does not.
    /// </summary>
    public ChartContext? Context { get; set; }

    /// <summary>Value transitions.</summary>
    public AnimationOptions? Animations { get; set; }
}

/// <summary>
/// Value transitions. Data updates tween fills, radii and stroke widths; camera-driven geometry
/// never animates, because it is written per frame while panning.
/// </summary>
/// <remarks>
/// Past a few thousand marks the engine degrades on its own: geometry stops animating first, then
/// everything, because dropped frames read as a bug while a simpler transition reads as restraint.
/// <c>prefers-reduced-motion</c> disables all of it.
/// </remarks>
public class AnimationOptions
{
    /// <summary>Whether value transitions run at all. Default true.</summary>
    public bool? Enabled { get; set; }

    /// <summary>How long a transition takes: a preset, or milliseconds.</summary>
    public AnimationSpeed? Speed { get; set; }

    /// <summary>
    /// Fade the mark layers in on first paint. Defaults on only under
    /// <see cref="ChartContext.Story"/>.
    /// </summary>
    public bool? Entrance { get; set; }
}

/// <summary>Geometry, projection and the opening view.</summary>
public class GeoOptions
{
    /// <summary>
    /// Where geometry comes from: a registry id such as <c>"world/countries@110m"</c> (see
    /// <see cref="GeoMaps"/>), a URL, or geometry in hand.
    /// </summary>
    public MapSource? Map { get; set; }

    /// <summary>Which object to read, when a TopoJSON topology holds several.</summary>
    public string? Object { get; set; }

    /// <summary>
    /// Geometry property holding each feature's join key. Detected when omitted, and registry
    /// packs carry a recommended one, which is usually the right answer.
    /// </summary>
    public string? KeyField { get; set; }

    /// <summary>Geometry property holding each feature's display name.</summary>
    public string? NameField { get; set; }

    /// <summary>
    /// Normalise ring winding on ingest. Default true, and worth leaving on: a counterclockwise
    /// ring makes d3-geo render the whole sphere minus your polygon, which throws nothing and
    /// looks like a solid fill.
    /// </summary>
    public bool? RepairWinding { get; set; }

    /// <summary>
    /// The projection: a name from <see cref="Projections"/>, or a spec with rotation, centre or
    /// standard parallels. Equal Earth by default; registry packs may recommend their own.
    /// </summary>
    public ProjectionSpec? Projection { get; set; }

    /// <summary>What the map opens framed on.</summary>
    public ViewOptions? View { get; set; }

    /// <summary>The lat/lon grid.</summary>
    public GraticuleOptions? Graticule { get; set; }

    /// <summary>The globe's disc, on a projection that has one.</summary>
    public SphereOptions? Sphere { get; set; }

    /// <summary>Fill for the basemap drawn when no feature series is configured.</summary>
    public string? Fill { get; set; }

    /// <summary>Disputed-territory policy. Declared by the core but not applied yet.</summary>
    public BoundaryPolicy? Boundaries { get; set; }
}

/// <summary>
/// A projection, by name or with its parameters. Assign a name from <see cref="Projections"/>
/// directly: <c>Projection = Projections.Mercator</c>.
/// </summary>
public class ProjectionSpec
{
    /// <summary>
    /// Projection name: one of <see cref="Projections"/>, or anything registered at runtime.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Rotation as <c>[lambda, phi]</c> or <c>[lambda, phi, gamma]</c>, in degrees. On a globe
    /// this is which way the sphere faces.
    /// </summary>
    public double[]? Rotate { get; set; }

    /// <summary>The point the projection is centred on.</summary>
    public LonLat? Center { get; set; }

    /// <summary>Standard parallels, for the conic projections.</summary>
    public double[]? Parallels { get; set; }

    /// <summary>Post-projection rotation of the plane, in degrees.</summary>
    public double? Angle { get; set; }

    /// <summary>Small-circle clip angle, in degrees. What hides a globe's far side.</summary>
    public double? ClipAngle { get; set; }

    /// <summary>Mirror the x axis.</summary>
    public bool? ReflectX { get; set; }

    /// <summary>Mirror the y axis.</summary>
    public bool? ReflectY { get; set; }

    /// <summary>A projection by name.</summary>
    public static implicit operator ProjectionSpec(string name) => new() { Name = name };
}

/// <summary>What the map opens framed on.</summary>
public class ViewOptions
{
    /// <summary>
    /// The frame: the data, the world, nothing, or an explicit lon/lat box. Data by default.
    /// </summary>
    public ViewFit? Fit { get; set; }

    /// <summary>Space between the framed geometry and the plot's edge. Default 16.</summary>
    public Padding? Padding { get; set; }
}

/// <summary>The lat/lon grid.</summary>
public class GraticuleOptions
{
    /// <summary>Draw it. Default false.</summary>
    public bool? Show { get; set; }

    /// <summary>Degrees between lines. Default 10.</summary>
    public double? Step { get; set; }

    /// <summary>Line colour.</summary>
    public string? Color { get; set; }

    /// <summary>Line width in pixels.</summary>
    public double? Width { get; set; }
}

/// <summary>The globe's disc, on a projection that has one.</summary>
public class SphereOptions
{
    /// <summary>Draw it. Default false.</summary>
    public bool? Show { get; set; }

    /// <summary>Disc fill: the ocean, in effect.</summary>
    public string? Fill { get; set; }

    /// <summary>Disc outline colour.</summary>
    public string? Stroke { get; set; }

    /// <summary>Disc outline width in pixels.</summary>
    public double? Width { get; set; }
}

/// <summary>How a numeric range becomes colour.</summary>
public class ScaleOptions
{
    /// <summary>
    /// Classification. Quantile by default, so every class is populated and the break values are
    /// printed: classification silently changes a choropleth's conclusion.
    /// </summary>
    public ScaleType? Type { get; set; }

    /// <summary>Number of classes, for a classed scale. Default 5.</summary>
    public int? Classes { get; set; }

    /// <summary>
    /// A palette name from <see cref="Palettes"/>, or an explicit colour list, which is taken
    /// literally rather than resampled.
    /// </summary>
    public PaletteSpec? Palette { get; set; }

    /// <summary>Fix the input range instead of taking it from the data.</summary>
    public double[]? Domain { get; set; }

    /// <summary>Explicit break values. Required by <see cref="ScaleType.Threshold"/>.</summary>
    public double[]? Breaks { get; set; }

    /// <summary>Reverse the palette.</summary>
    public bool? Reverse { get; set; }

    /// <summary>Round the domain outward. Default true for continuous scales.</summary>
    public bool? Nice { get; set; }

    /// <summary>
    /// Colour for features with no value. Its own colour on purpose: an unmatched feature falling
    /// into the lowest class would understate it.
    /// </summary>
    public string? NullColor { get; set; }

    /// <summary>Legend label for the no-data class. Default "No data".</summary>
    public string? NullLabel { get; set; }
}

/// <summary>How a magnitude becomes a radius, or a line width.</summary>
public class SizeOptions
{
    /// <summary>
    /// Field or accessor supplying the magnitude. Defaults to the series' own value field.
    /// </summary>
    public FieldRef? Field { get; set; }

    /// <summary>
    /// Sqrt by default, and deliberately: radius proportional to value makes a circle's area grow
    /// with the square of it, which overstates large values by a wide margin.
    /// </summary>
    public SizeScaleType? Scale { get; set; }

    /// <summary>Output range in screen pixels, <c>[min, max]</c>.</summary>
    public double[]? Range { get; set; }

    /// <summary>Fix the input range instead of taking it from the data.</summary>
    public double[]? Domain { get; set; }
}

/// <summary>Line or outline styling.</summary>
public class StrokeOptions
{
    /// <summary>Stroke colour.</summary>
    public string? Color { get; set; }

    /// <summary>Stroke width in pixels.</summary>
    public double? Width { get; set; }

    /// <summary>Stroke opacity, 0 to 1.</summary>
    public double? Opacity { get; set; }

    /// <summary>SVG dash pattern, such as <c>"4 2"</c>.</summary>
    public string? DashArray { get; set; }
}

/// <summary>The scale, printed.</summary>
public class LegendOptions
{
    /// <summary>Draw it. Default true when a series has a scale.</summary>
    public bool? Show { get; set; }

    /// <summary>
    /// Which side of the plot it sits on. A side makes it a column: the plot is measured against
    /// what is left of the container, and a gradient bar turns vertical.
    /// </summary>
    public LegendPosition? Position { get; set; }

    /// <summary>Column width in pixels, for a legend on the left or right. Default 180.</summary>
    public double? Width { get; set; }

    /// <summary>Alignment along the legend's own axis.</summary>
    public LegendAlign? Align { get; set; }

    /// <summary>Legend title. Defaults to the series name.</summary>
    public string? Title { get; set; }

    /// <summary>Click a class to mute it. Default true.</summary>
    public bool? Interactive { get; set; }

    /// <summary>Include the no-data class. Default true.</summary>
    public bool? ShowNull { get; set; }

    /// <summary>Swatches or one continuous bar. Decided from the scale by default.</summary>
    public LegendStyle? Style { get; set; }

    /// <summary>
    /// Called with <c>(item, index)</c> for each legend entry, returning its label.
    /// </summary>
    public JsFunction? Formatter { get; set; }

    /// <summary>Called with each number printed under a gradient bar.</summary>
    public JsFunction? TickFormatter { get; set; }

    /// <summary>
    /// The arrow that rides a gradient bar and tracks the hovered feature, so a reader sees where
    /// it falls on the scale instead of matching colours by eye. Gradient legends only, on by
    /// default.
    /// </summary>
    public LegendMarkerOptions? Marker { get; set; }
}

/// <summary>The arrow that tracks the hovered feature along a gradient legend.</summary>
public class LegendMarkerOptions
{
    /// <summary>Show the arrow. Default true.</summary>
    public bool? Show { get; set; }

    /// <summary>Show the value above the arrow. Default true.</summary>
    public bool? Label { get; set; }

    /// <summary>Turn the marker on or off.</summary>
    public static implicit operator LegendMarkerOptions(bool show) => new() { Show = show };
}

/// <summary>What hovering a mark says.</summary>
public class TooltipOptions
{
    /// <summary>Show tooltips. Default true.</summary>
    public bool? Enabled { get; set; }

    /// <summary>Track the pointer rather than anchoring to the mark.</summary>
    public bool? FollowCursor { get; set; }

    /// <summary>
    /// Called with the hovered mark's context, returning HTML. Responsible for its own escaping.
    /// Return an empty string to show nothing: the formatter also runs for background geometry,
    /// which has no data row behind it.
    /// </summary>
    public JsFunction? Formatter { get; set; }

    /// <summary>Called with the value, returning the text that stands in for it.</summary>
    public JsFunction? ValueFormatter { get; set; }

    /// <summary>Offset from the anchor, <c>[x, y]</c> in pixels.</summary>
    public double[]? Offset { get; set; }
}

/// <summary>Values written onto the map itself.</summary>
public class DataLabelOptions
{
    /// <summary>Draw labels. Default false.</summary>
    public bool? Enabled { get; set; }

    /// <summary>Field or accessor supplying the label text. Defaults to the feature's name.</summary>
    public FieldRef? Field { get; set; }

    /// <summary>
    /// Called with <c>{ value, name, key }</c>, returning the label text.
    /// </summary>
    public JsFunction? Formatter { get; set; }

    /// <summary>What happens to a label that would overlap another. Hide by default.</summary>
    public LabelCollision? Collision { get; set; }

    /// <summary>Skip labels for features smaller than this many square pixels.</summary>
    public double? MinFeatureArea { get; set; }

    /// <summary>Label typography and halo.</summary>
    public DataLabelStyle? Style { get; set; }
}

/// <summary>Label typography and halo.</summary>
public class DataLabelStyle
{
    /// <summary>Font size in pixels.</summary>
    public double? FontSize { get; set; }

    /// <summary>Font weight, as a number or a CSS keyword.</summary>
    public object? FontWeight { get; set; }

    /// <summary>Draw a halo, so a label stays readable over any class. Default true.</summary>
    public bool? Halo { get; set; }

    /// <summary>Halo colour.</summary>
    public string? HaloColor { get; set; }

    /// <summary>Halo width in pixels.</summary>
    public double? HaloWidth { get; set; }
}

/// <summary>How hover, selection and muting look.</summary>
public class StatesOptions
{
    /// <summary>What a hovered mark looks like.</summary>
    public HoverState? Hover { get; set; }

    /// <summary>What a selected mark looks like.</summary>
    public ActiveState? Active { get; set; }

    /// <summary>What everything else looks like while something is selected or filtered.</summary>
    public MutedState? Muted { get; set; }
}

/// <summary>What a hovered mark looks like.</summary>
public class HoverState
{
    /// <summary>React to hover at all. Default true.</summary>
    public bool? Enabled { get; set; }

    /// <summary>Lightening applied to the fill, 0 to 1.</summary>
    public double? Brightness { get; set; }

    /// <summary>Outline colour while hovered.</summary>
    public string? Stroke { get; set; }

    /// <summary>Outline width while hovered.</summary>
    public double? StrokeWidth { get; set; }
}

/// <summary>What a selected mark looks like.</summary>
public class ActiveState
{
    /// <summary>Draw a selected state at all. Default true.</summary>
    public bool? Enabled { get; set; }

    /// <summary>Outline colour while selected.</summary>
    public string? Stroke { get; set; }

    /// <summary>Outline width while selected.</summary>
    public double? StrokeWidth { get; set; }
}

/// <summary>What everything else looks like while something is selected or filtered.</summary>
public class MutedState
{
    /// <summary>Opacity of the marks that are not selected. Default 0.25.</summary>
    public double? Opacity { get; set; }
}

/// <summary>Light or dark chrome, and the categorical palette series draw from.</summary>
public class ThemeOptions
{
    /// <summary>
    /// Which palette the map's chrome follows. The dark palette is self-contained and always
    /// legible, so it does not take the page's <c>--apx-*</c> tokens; override
    /// <c>--apexmaps-*</c> under <c>.apexmaps--dark</c> to brand it.
    /// </summary>
    public ThemeMode? Mode { get; set; }

    /// <summary>Default categorical palette, for series that do not name one.</summary>
    public string? Palette { get; set; }
}

/// <summary>Zoom, pan, globe rotation, selection and hit assistance.</summary>
public class InteractionOptions
{
    /// <summary>Scale changes: the wheel, the buttons, a double-click, a pinch.</summary>
    public ZoomOptions? Zoom { get; set; }

    /// <summary>Dragging the map around.</summary>
    public PanOptions? Pan { get; set; }

    /// <summary>
    /// Spin the sphere on drag instead of panning the plane. Auto by default, which means on for
    /// globe projections and off for flat ones, where a drag means "move the map".
    /// </summary>
    public RotateOptions? Rotate { get; set; }

    /// <summary>Clicking and box-dragging marks into a selection.</summary>
    public SelectionOptions? Selection { get; set; }

    /// <summary>
    /// Proximity hit assistance for point marks: a pointer within a few pixels of a bubble or
    /// marker hovers and clicks it as if it were on it, so a 3px bubble does not demand a 3px hit.
    /// </summary>
    public NearestOptions? Nearest { get; set; }
}

/// <summary>Scale changes.</summary>
public class ZoomOptions
{
    /// <summary>Allow zooming at all. Default true.</summary>
    public bool? Enabled { get; set; }

    /// <summary>Furthest out the camera may go, as a scale factor. Default 1.</summary>
    public double? Min { get; set; }

    /// <summary>Closest in the camera may go. Default 40.</summary>
    public double? Max { get; set; }

    /// <summary>Zoom on the wheel, anchored under the cursor. Default true.</summary>
    public bool? Wheel { get; set; }

    /// <summary>Zoom in on a double-click. Default true.</summary>
    public bool? DoubleClick { get; set; }

    /// <summary>
    /// Scale factor per step, used by the buttons, the keyboard and a double-click. Default 1.6.
    /// </summary>
    public double? Step { get; set; }

    /// <summary>
    /// The on-screen <c>+</c> / <c>-</c> group. Rendered by default wherever zoom is enabled,
    /// because every other way to change scale is a gesture, and a gesture is no keyboard path.
    /// </summary>
    public ZoomControlsOptions? Controls { get; set; }
}

/// <summary>The on-screen zoom controls.</summary>
public class ZoomControlsOptions
{
    /// <summary>Render the group. Default true wherever zoom is enabled.</summary>
    public bool? Show { get; set; }

    /// <summary>Which corner of the plot it sits in. Top right by default.</summary>
    public ZoomControlsPosition? Position { get; set; }

    /// <summary>
    /// Include the reset control. Default true: at the default step, eight clicks of <c>+</c> pass
    /// 40x, and no gesture takes the reader back to the whole map in one move.
    /// </summary>
    public bool? Reset { get; set; }

    /// <summary>Turn the controls on or off.</summary>
    public static implicit operator ZoomControlsOptions(bool show) => new() { Show = show };
}

/// <summary>Dragging the map around.</summary>
public class PanOptions
{
    /// <summary>Allow panning. Default true.</summary>
    public bool? Enabled { get; set; }

    /// <summary>Carry momentum after the drag ends. Default true.</summary>
    public bool? Inertia { get; set; }
}

/// <summary>Spinning the sphere instead of panning the plane.</summary>
public class RotateOptions
{
    /// <summary>
    /// On, off, or auto. Auto is the default: on for globe projections, off for flat ones. Forcing
    /// it on opts a stereographic or azimuthal view into the same gesture.
    /// </summary>
    public AutoBool? Enabled { get; set; }

    /// <summary>Glide on after the drag ends. Defaults to the pan setting.</summary>
    public bool? Inertia { get; set; }
}

/// <summary>Clicking and box-dragging marks into a selection.</summary>
public class SelectionOptions
{
    /// <summary>Allow selection. Default true.</summary>
    public bool? Enabled { get; set; }

    /// <summary>Allow more than one mark to be selected. Default false.</summary>
    public bool? Multiple { get; set; }

    /// <summary>
    /// Drag a box to select everything inside it. Default true, since it costs nothing when
    /// unused: a plain drag still pans.
    /// </summary>
    public bool? Rectangle { get; set; }

    /// <summary>
    /// The modifier that turns a drag into a selection box. Shift by default.
    /// <see cref="SelectionModifier.None"/> makes every drag a box, and so requires panning off.
    /// </summary>
    public SelectionModifier? Modifier { get; set; }
}

/// <summary>Proximity hit assistance for point marks.</summary>
public class NearestOptions
{
    /// <summary>Enable it. Default true.</summary>
    public bool? Enabled { get; set; }

    /// <summary>How far from a mark still counts as on it, in screen pixels. Default 12.</summary>
    public double? Radius { get; set; }
}

/// <summary>The accessibility layer. On by default, and free in every tier.</summary>
public class A11yOptions
{
    /// <summary>Enable it. Default true.</summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// The map's description. <c>"auto"</c>, the default, generates one from the spec and the data.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>Accessible name for the map region.</summary>
    public string? Label { get; set; }

    /// <summary>Render the data as a visually hidden table. Default false.</summary>
    public bool? DataTable { get; set; }

    /// <summary>
    /// Make individual features keyboard-reachable below this count, above which the map is one
    /// stop rather than three thousand. Default 500.
    /// </summary>
    public int? KeyboardFeatureLimit { get; set; }
}

/// <summary>A cross-filter group.</summary>
public class LinkOptions
{
    /// <summary>
    /// Group name. Maps naming the same group share their selection. Keys have to mean the same
    /// thing across the group, which they do whenever the maps are of the same geography.
    /// </summary>
    public string? Group { get; set; }

    /// <summary>Which way the selection travels. Bidirectional by default.</summary>
    public LinkFilter? Filter { get; set; }
}

/// <summary>Development diagnostics.</summary>
public class DebugOptions
{
    /// <summary>
    /// Print diagnostics. Auto by default, which means on for a localhost origin.
    /// </summary>
    public AutoBool? Enabled { get; set; }

    /// <summary>
    /// Report what a join actually matched, with suggestions for the rows that did not. Around
    /// nine in ten real map failures are join failures.
    /// </summary>
    public bool? JoinDiagnostics { get; set; }
}

/// <summary>An override applied below a container width.</summary>
public class ResponsiveRule
{
    /// <summary>Container width, in pixels, below which the rule applies.</summary>
    public double Breakpoint { get; set; }

    /// <summary>Options merged over the base at that width.</summary>
    public MapsOptions? Options { get; set; }
}
