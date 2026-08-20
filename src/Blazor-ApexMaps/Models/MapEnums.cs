using System.Text.Json.Serialization;

namespace Blazor_ApexMaps.Models;

// The serializer registers a JsonStringEnumConverter with a camelCase policy, so single-word
// members serialize to their lowercase form (Quantile -> "quantile", EqualEarth -> "equalEarth").
// Members whose wire form is not a camelCase word (hyphenated positions, EPSG codes) carry a
// [JsonStringEnumMemberName] attribute, which that converter honours.

/// <summary>Which colour palette the map's own chrome follows.</summary>
public enum ThemeMode
{
    /// <summary>Light chrome.</summary>
    Light,

    /// <summary>Dark chrome. Self-contained, so it stays legible whatever the page's tokens say.</summary>
    Dark,

    /// <summary>Follow the reader's <c>prefers-color-scheme</c>.</summary>
    Auto,
}

/// <summary>
/// What the chart is for. <c>Story</c> animates entrances; <c>Dashboard</c> does not, because a
/// dashboard reader wants the number now rather than a performance.
/// </summary>
public enum ChartContext
{
    /// <summary>No entrance animation. The default.</summary>
    Dashboard,

    /// <summary>Animate entrances. A licensed feature: watermarked without a key.</summary>
    Story,
}

/// <summary>Preset transition speeds for value changes.</summary>
public enum AnimationSpeedPreset
{
    /// <summary>Slow.</summary>
    Slow,

    /// <summary>Normal.</summary>
    Normal,

    /// <summary>Fast.</summary>
    Fast,

    /// <summary>No transition at all.</summary>
    Instant,
}

/// <summary>What the opening view frames.</summary>
public enum ViewFitMode
{
    /// <summary>The geometry the series cover. The default.</summary>
    Data,

    /// <summary>The whole world.</summary>
    World,

    /// <summary>Nothing: leave the camera where the projection put it.</summary>
    None,
}

/// <summary>How a numeric range is turned into colours.</summary>
public enum ScaleType
{
    /// <summary>Equal-count classes. The default, because every class is populated.</summary>
    Quantile,

    /// <summary>Equal-width classes over the domain.</summary>
    Quantize,

    /// <summary>Equal-width classes. A synonym for <see cref="Quantize"/>.</summary>
    EqualInterval,

    /// <summary>Jenks natural breaks: minimises within-class variance.</summary>
    Jenks,

    /// <summary>Natural breaks. A synonym for <see cref="Jenks"/>.</summary>
    NaturalBreaks,

    /// <summary>Explicit breaks, supplied through <see cref="ScaleOptions.Breaks"/>.</summary>
    Threshold,

    /// <summary>Continuous linear ramp.</summary>
    Linear,

    /// <summary>Continuous logarithmic ramp, for values spanning orders of magnitude.</summary>
    Log,

    /// <summary>Continuous square-root ramp.</summary>
    Sqrt,

    /// <summary>Categorical: one colour per distinct value, with no implied order.</summary>
    Ordinal,
}

/// <summary>How a magnitude becomes a radius or a line width.</summary>
public enum SizeScaleType
{
    /// <summary>
    /// Radius proportional to the square root of the value, so a circle's area carries the
    /// quantity. The default, and the correct one.
    /// </summary>
    Sqrt,

    /// <summary>Radius proportional to the value, which overstates large values by area.</summary>
    Linear,

    /// <summary>Logarithmic, for values spanning orders of magnitude.</summary>
    Log,
}

/// <summary>Which of the five series types a series is.</summary>
public enum SeriesType
{
    /// <summary>Areas coloured by value.</summary>
    Choropleth,

    /// <summary>Proportional symbols, sized by value.</summary>
    Bubble,

    /// <summary>Points of interest, at a fixed size.</summary>
    Marker,

    /// <summary>Great-circle connections between two endpoints.</summary>
    Arc,

    /// <summary>Routes through the vertices they are given.</summary>
    Line,
}

/// <summary>Built-in marker shapes, each drawn from a generated path.</summary>
public enum MarkerShape
{
    /// <summary>A circle.</summary>
    Circle,

    /// <summary>A square.</summary>
    Square,

    /// <summary>A diamond.</summary>
    Diamond,

    /// <summary>A triangle.</summary>
    Triangle,

    /// <summary>A five-pointed star.</summary>
    Star,

    /// <summary>A cross.</summary>
    Cross,

    /// <summary>The map teardrop, anchored at its point rather than its centre.</summary>
    Pin,
}

/// <summary>Built-in pattern tiles for a patterned fill.</summary>
public enum PatternType
{
    /// <summary>Dots. The default.</summary>
    Dots,

    /// <summary>Filled squares.</summary>
    Squares,

    /// <summary>A checkerboard.</summary>
    Checks,

    /// <summary>Horizontal lines.</summary>
    Lines,

    /// <summary>A square grid.</summary>
    Grid,

    /// <summary>Diagonal lines.</summary>
    Diagonal,

    /// <summary>Crossed diagonals.</summary>
    Crosshatch,

    /// <summary>A tile you supply, through <see cref="PatternFillOptions.Path"/>.</summary>
    Custom,
}

/// <summary>How an image sits inside a feature's bounding box.</summary>
public enum ImageFit
{
    /// <summary>Fill the box and crop the overflow. The default.</summary>
    Cover,

    /// <summary>Fit the whole image inside the box.</summary>
    Contain,

    /// <summary>Stretch to the box, ignoring the aspect ratio.</summary>
    Fill,
}

/// <summary>How the child level of a drilldown is scoped to the clicked parent.</summary>
public enum DrilldownScope
{
    /// <summary>
    /// Detect it: a child property holding the parent's key, else a key prefix. The default, and
    /// enough for nearly all published hierarchical geometry.
    /// </summary>
    Auto,

    /// <summary>A child property holding the parent's key.</summary>
    Property,

    /// <summary>A key prefix, as county FIPS <c>06037</c> sits under state <c>06</c>.</summary>
    KeyPrefix,

    /// <summary>No scoping: draw the whole child map.</summary>
    All,
}

/// <summary>How a drilldown level swap is animated.</summary>
public enum DrilldownAnimation
{
    /// <summary>
    /// Frame the clicked feature and develop the child level out of the parent's own colour, so
    /// the geography never moves across the swap. The default.
    /// </summary>
    Zoom,

    /// <summary>Swap with no motion. Also what a reader who asked for reduced motion gets.</summary>
    None,
}

/// <summary>How a mark's entrance is drawn.</summary>
public enum MarkAnimationType
{
    /// <summary>Symbols grow from their anchor.</summary>
    Grow,

    /// <summary>Paths draw along their length.</summary>
    Draw,

    /// <summary>Either fades in.</summary>
    Fade,

    /// <summary>No entrance.</summary>
    None,
}

/// <summary>How flow beads read along a route.</summary>
public enum FlowStyle
{
    /// <summary>Beads travelling the route: the one that reads as traffic. The default.</summary>
    Dots,

    /// <summary>A dashed highlight marching along the route.</summary>
    Dash,
}

/// <summary>Whether flow beads belong to the ground or to the screen.</summary>
public enum FlowScale
{
    /// <summary>Anchored to the route, so a bead stays over the same stretch of it. The default.</summary>
    Zoom,

    /// <summary>Fixed size and spacing however far the reader zooms.</summary>
    Screen,
}

/// <summary>Which side of the plot the legend sits on.</summary>
public enum LegendPosition
{
    /// <summary>Below the plot. The default.</summary>
    Bottom,

    /// <summary>Above the plot.</summary>
    Top,

    /// <summary>A column to the left, measured out of the container's width.</summary>
    Left,

    /// <summary>A column to the right, measured out of the container's width.</summary>
    Right,
}

/// <summary>How the legend's items are aligned along its axis.</summary>
public enum LegendAlign
{
    /// <summary>Packed to the start.</summary>
    Start,

    /// <summary>Centred.</summary>
    Center,

    /// <summary>Packed to the end.</summary>
    End,
}

/// <summary>How the legend draws its scale.</summary>
public enum LegendStyle
{
    /// <summary>Classes for a classed scale, a gradient bar for a continuous one. The default.</summary>
    Auto,

    /// <summary>One swatch per class.</summary>
    Classes,

    /// <summary>One continuous bar. Classed scales draw as hard bands on it.</summary>
    Gradient,
}

/// <summary>What happens to a generated label that would overlap another.</summary>
public enum LabelCollision
{
    /// <summary>Drop it. The default.</summary>
    Hide,

    /// <summary>Draw it anyway.</summary>
    None,
}

/// <summary>Which corner of the plot the zoom controls sit in.</summary>
public enum ZoomControlsPosition
{
    /// <summary>Top left.</summary>
    [JsonStringEnumMemberName("top-left")]
    TopLeft,

    /// <summary>Top right. The default.</summary>
    [JsonStringEnumMemberName("top-right")]
    TopRight,

    /// <summary>Bottom left.</summary>
    [JsonStringEnumMemberName("bottom-left")]
    BottomLeft,

    /// <summary>Bottom right.</summary>
    [JsonStringEnumMemberName("bottom-right")]
    BottomRight,
}

/// <summary>The modifier key that turns a drag into a selection box.</summary>
public enum SelectionModifier
{
    /// <summary>Shift. The default, and the convention.</summary>
    Shift,

    /// <summary>Alt or Option.</summary>
    Alt,

    /// <summary>Command or the Windows key.</summary>
    Meta,

    /// <summary>Control.</summary>
    Ctrl,

    /// <summary>
    /// No modifier: every drag is a selection box. Requires panning to be turned off, since one
    /// gesture cannot mean both.
    /// </summary>
    None,
}

/// <summary>Which way a linked group's selection travels.</summary>
public enum LinkFilter
{
    /// <summary>Send and receive. The default.</summary>
    Bidirectional,

    /// <summary>Send without receiving.</summary>
    Emit,

    /// <summary>Follow without leading.</summary>
    Receive,
}

/// <summary>Whose view of a disputed boundary a map carries.</summary>
public enum BoundaryPolicy
{
    /// <summary>Boundaries as administered on the ground.</summary>
    [JsonStringEnumMemberName("de-facto")]
    DeFacto,

    /// <summary>The United Nations view.</summary>
    [JsonStringEnumMemberName("un")]
    Un,

    /// <summary>Disputed boundaries drawn dashed, taking no side.</summary>
    [JsonStringEnumMemberName("neutral-dashed")]
    NeutralDashed,

    /// <summary>Draw no disputed boundaries.</summary>
    None,
}

/// <summary>Where an annotation's label chip sits relative to its anchor.</summary>
public enum AnnotationLabelPosition
{
    /// <summary>Above the anchor. The default.</summary>
    Top,

    /// <summary>Below the anchor.</summary>
    Bottom,

    /// <summary>Left of the anchor.</summary>
    Left,

    /// <summary>Right of the anchor.</summary>
    Right,

    /// <summary>On the anchor.</summary>
    Center,
}

/// <summary>How a camera move is animated.</summary>
public enum CameraTransition
{
    /// <summary>
    /// A Van Wijk zoom-and-pan path, with its duration derived from the distance. The default,
    /// and the narrative one.
    /// </summary>
    Fly,

    /// <summary>A fixed-duration, mechanical move.</summary>
    Ease,

    /// <summary>No animation.</summary>
    Jump,
}
