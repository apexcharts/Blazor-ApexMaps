namespace Blazor_ApexMaps.Models;

/// <summary>
/// Where the camera should end up. A centre, a zoom, or both.
/// </summary>
/// <remarks>
/// On an azimuthal projection a move to a place turns the sphere rather than panning a plane, so
/// the same target works on a globe and on a flat map.
/// </remarks>
public class CameraTarget
{
    /// <summary>The geographic point to put at the centre of the plot.</summary>
    public LonLat? Center { get; set; }

    /// <summary>The scale to land at, 1 being the opening fit.</summary>
    public double? Zoom { get; set; }

    /// <summary>Space to leave around the framed geometry.</summary>
    public Padding? Padding { get; set; }

    /// <summary>Cap on the scale this move may reach.</summary>
    public double? MaxZoom { get; set; }

    /// <summary>
    /// Duration in milliseconds. Left unset, a fly derives it from the distance, so crossing a
    /// continent takes longer than nudging to a neighbouring county.
    /// </summary>
    public double? Duration { get; set; }

    /// <summary>Easing name, for an eased move.</summary>
    public string? Ease { get; set; }

    /// <summary>Speed multiplier for a fly. Higher is faster.</summary>
    public double? Speed { get; set; }

    /// <summary>
    /// Van Wijk curvature for a fly. Higher arcs out further; 0 makes the path a straight
    /// zoom-and-pan.
    /// </summary>
    public double? Curve { get; set; }
}

/// <summary>How a move that frames something is animated.</summary>
public class FrameOptions
{
    /// <summary>Space to leave around the framed geometry. Default 24.</summary>
    public Padding? Padding { get; set; }

    /// <summary>Duration in milliseconds.</summary>
    public double? Duration { get; set; }

    /// <summary>Fly, ease, or jump. Fly by default.</summary>
    public CameraTransition? Transition { get; set; }
}

/// <summary>How the current view leaves the page.</summary>
public class MapExportOptions
{
    /// <summary>
    /// Painted behind the map. SVG defaults to transparent, like the live chart; PNG defaults to
    /// the container's own background, so a dark-mode screenshot does not arrive as pale strokes
    /// on nothing.
    /// </summary>
    public string? Background { get; set; }

    /// <summary>PNG pixel-density multiplier. Default 2, which survives print and retina.</summary>
    public double? Scale { get; set; }

    /// <summary>Filename without its extension. Defaults to the map id.</summary>
    public string? Filename { get; set; }
}

/// <summary>Where the camera is now.</summary>
public class CameraState
{
    /// <summary>Scale, 1 at the opening fit.</summary>
    public double K { get; set; }

    /// <summary>World-space x translation, in pixels.</summary>
    public double X { get; set; }

    /// <summary>World-space y translation, in pixels.</summary>
    public double Y { get; set; }
}

/// <summary>Provenance for a geometry pack, and what the core recommends drawing it with.</summary>
public class MapMeta
{
    /// <summary>Where the boundaries came from, e.g. "Natural Earth 5.1.1".</summary>
    public string? Source { get; set; }

    /// <summary>The data's licence, which is not the library's licence.</summary>
    public string? License { get; set; }

    /// <summary>Attribution text to display. Empty for public-domain sources.</summary>
    public string? Attribution { get; set; }

    /// <summary>Boundary vintage, e.g. "2024".</summary>
    public string? Vintage { get; set; }

    /// <summary>Geometry detail: low, medium or high.</summary>
    public string? Detail { get; set; }

    /// <summary>Whose view of disputed boundaries this pack encodes.</summary>
    public string? Boundaries { get; set; }

    /// <summary>The join key this pack recommends.</summary>
    public string? KeyField { get; set; }

    /// <summary>What one feature in this pack is, e.g. "Prefectures".</summary>
    public string? LevelName { get; set; }

    /// <summary>The canonical pack id this one is an alias of, when it is an alias.</summary>
    public string? AliasOf { get; set; }
}
