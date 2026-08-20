using System.Text.Json;

namespace Blazor_ApexMaps.Models;

// Event payloads arrive from the browser as JSON, so the parts of them whose shape is yours (the
// data row, the geometry properties) arrive as JsonElement rather than as a guess. Read<T>() turns
// a row back into your own model when you want it typed.

/// <summary>
/// What a mark event says: which feature or mark, what it was worth, and the row behind it.
/// </summary>
public class FeatureEventArgs
{
    /// <summary>Join key of the feature, or the mark's own key within its series.</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Display name, when the geometry or the row carried one.</summary>
    public string? Name { get; set; }

    /// <summary>The value, or null where the feature had no data.</summary>
    public double? Value { get; set; }

    /// <summary>The data row behind the mark, when it had one.</summary>
    public JsonElement? Datum { get; set; }

    /// <summary>The geometry feature's properties, for a feature mark.</summary>
    public Dictionary<string, JsonElement>? Properties { get; set; }

    /// <summary>Name of the series the mark belongs to.</summary>
    public string? SeriesName { get; set; }

    /// <summary>Index of the series the mark belongs to.</summary>
    public int SeriesIndex { get; set; }

    /// <summary>The data row as one of your own models, or null when there was no row.</summary>
    public T? Read<T>(JsonSerializerOptions? options = null)
        => Datum is null || Datum.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
            ? default
            : Datum.Value.Deserialize<T>(options ?? Serialization.MapsSerializer.Default);

    /// <summary>A geometry property as a string, or null when the feature has no such property.</summary>
    public string? Property(string name)
        => Properties is not null && Properties.TryGetValue(name, out var value)
            ? value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString()
            : null;
}

/// <summary>A deeper level is on screen. Raised after it renders, so a handler can load its data.</summary>
public class DrilldownEventArgs
{
    /// <summary>Join key of the feature that was drilled into.</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Its display name.</summary>
    public string? Name { get; set; }

    /// <summary>The map left behind.</summary>
    public string? From { get; set; }

    /// <summary>The map now displayed.</summary>
    public string? To { get; set; }

    /// <summary>How deep the map now is. 1 is the first level below the top.</summary>
    public int Depth { get; set; }

    /// <summary>How many child features the level was scoped to.</summary>
    public int FeatureCount { get; set; }
}

/// <summary>The map climbed back out of a drilldown level.</summary>
public class DrillupEventArgs
{
    /// <summary>The map now displayed.</summary>
    public string? To { get; set; }

    /// <summary>How deep the map now is. 0 is the top.</summary>
    public int Depth { get; set; }
}

/// <summary>The selection changed, here or on a linked map.</summary>
public class SelectionChangeEventArgs
{
    /// <summary>Keys of the selected features.</summary>
    public List<string> Ids { get; set; } = [];

    /// <summary>
    /// What changed it: a click, a rectangle, the API, or a linked map in the same group.
    /// </summary>
    public string? Source { get; set; }
}

/// <summary>A legend class was clicked, muting or unmuting it.</summary>
public class LegendToggleEventArgs
{
    /// <summary>Index of the class on its scale.</summary>
    public int ClassIndex { get; set; }
}

/// <summary>The camera's scale changed.</summary>
public class ZoomEventArgs
{
    /// <summary>The new scale, 1 at the opening fit.</summary>
    public double K { get; set; }
}

/// <summary>The plot was resized.</summary>
public class ResizeEventArgs
{
    /// <summary>New plot width in pixels.</summary>
    public double Width { get; set; }

    /// <summary>New plot height in pixels.</summary>
    public double Height { get; set; }
}

/// <summary>The globe turned.</summary>
public class RotateEventArgs
{
    /// <summary>The projection's rotation, <c>[lambda, phi, gamma]</c> in degrees.</summary>
    public double[] Rotate { get; set; } = [];
}

/// <summary>
/// What a join actually matched, and what it did not. Around nine in ten real map failures are
/// join failures, and every library renders them as silent grey.
/// </summary>
public class JoinDiagnostics
{
    /// <summary>How many data rows found a feature.</summary>
    public int Matched { get; set; }

    /// <summary>How many rows there were.</summary>
    public int TotalData { get; set; }

    /// <summary>How many features there were.</summary>
    public int TotalFeatures { get; set; }

    /// <summary>Geometry property the join used.</summary>
    public string? GeoKeyField { get; set; }

    /// <summary>Data property the join used.</summary>
    public string? DataKeyField { get; set; }

    /// <summary>Rows that matched nothing, with the nearest feature names.</summary>
    public List<UnmatchedRow> UnmatchedData { get; set; } = [];

    /// <summary>Features that got no row, and so rendered as no-data.</summary>
    public List<UnmatchedFeature> UnmatchedFeatures { get; set; } = [];

    /// <summary>
    /// Keys held by more than one feature. A property of published geometry rather than a mistake:
    /// Natural Earth gives Australia and two of its territories the same <c>iso_a3</c>, so one row
    /// legitimately colours all three.
    /// </summary>
    public List<SharedKey> SharedKeys { get; set; } = [];

    /// <summary>Fuzzy substitutions that were applied, so the convenience stays auditable.</summary>
    public List<string> Applied { get; set; } = [];

    /// <summary>The whole thing as the multi-line report the core prints in development.</summary>
    public string? Report { get; set; }
}

/// <summary>A data row that matched no feature.</summary>
public class UnmatchedRow
{
    /// <summary>The key the row carried.</summary>
    public string? Key { get; set; }

    /// <summary>The closest features by name, best first.</summary>
    public List<JoinSuggestion> Suggestions { get; set; } = [];
}

/// <summary>A feature the join might have meant.</summary>
public class JoinSuggestion
{
    /// <summary>The feature's key.</summary>
    public string? FeatureKey { get; set; }

    /// <summary>The feature's name.</summary>
    public string? Name { get; set; }

    /// <summary>How close the match is, 0 to 1.</summary>
    public double Score { get; set; }

    /// <summary>Why it is a candidate: an alias, a normalised form, zero-padding, or similarity.</summary>
    public string? Reason { get; set; }
}

/// <summary>A feature that got no data row.</summary>
public class UnmatchedFeature
{
    /// <summary>The feature's key.</summary>
    public string? Key { get; set; }

    /// <summary>The feature's name.</summary>
    public string? Name { get; set; }
}

/// <summary>A join key held by more than one feature.</summary>
public class SharedKey
{
    /// <summary>The key.</summary>
    public string? Key { get; set; }

    /// <summary>How many features hold it.</summary>
    public int Count { get; set; }

    /// <summary>Their names.</summary>
    public List<string> Names { get; set; } = [];
}
