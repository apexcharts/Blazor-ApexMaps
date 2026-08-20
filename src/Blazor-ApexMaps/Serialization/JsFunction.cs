using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor_ApexMaps.Serialization;

/// <summary>
/// A JavaScript function passed through to the core, for the options that take one:
/// tooltip and legend formatters, data-label formatters, value accessors, per-feature
/// pattern and image decisions, drilldown map resolution.
/// </summary>
/// <remarks>
/// <para>
/// These options are functions in the core because they run per mark, per frame, or during
/// projection: a round trip to .NET for each of a few thousand features would be a stutter
/// rather than a chart. So the source travels as a string and is compiled once, in the browser,
/// when the map is created.
/// </para>
/// <para>
/// Write either an arrow function or a <c>function</c> expression. The source is compiled with
/// <c>new Function</c>, so it runs in the global scope: it can see <c>window</c> and anything
/// your app put there, and it cannot see your C# variables. Interpolate the values it needs.
/// </para>
/// <example>
/// <code>
/// Tooltip = new TooltipOptions
/// {
///     Formatter = JsFunction.From("ctx => `&lt;b&gt;${ctx.name}&lt;/b&gt;: ${ctx.value ?? 'no data'}`"),
/// }
/// </code>
/// </example>
/// <para>
/// The source is evaluated as code. Never build it from untrusted input.
/// </para>
/// </remarks>
[JsonConverter(typeof(JsFunctionConverter))]
public sealed class JsFunction
{
    /// <summary>The marker property the interop bridge looks for when reviving a function.</summary>
    internal const string MarkerProperty = "__apexJsFn";

    /// <summary>JavaScript source for a function expression.</summary>
    public string Source { get; }

    /// <summary>Creates a function from JavaScript source.</summary>
    /// <param name="source">An arrow function or <c>function</c> expression.</param>
    public JsFunction(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentException("A JsFunction needs JavaScript source.", nameof(source));
        }
        Source = source;
    }

    /// <summary>Creates a function from JavaScript source.</summary>
    public static JsFunction From(string source) => new(source);

    /// <summary>Creates a function from JavaScript source.</summary>
    public static implicit operator JsFunction(string source) => new(source);

    /// <inheritdoc />
    public override string ToString() => Source;
}

/// <summary>
/// Writes a <see cref="JsFunction"/> as <c>{ "__apexJsFn": "&lt;source&gt;" }</c>, which the
/// interop bridge revives into a real function anywhere it appears in the options tree.
/// </summary>
public sealed class JsFunctionConverter : JsonConverter<JsFunction>
{
    /// <inheritdoc />
    public override JsFunction? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType == JsonTokenType.String) return new JsFunction(reader.GetString()!);

        using var doc = JsonDocument.ParseValue(ref reader);
        return doc.RootElement.TryGetProperty(JsFunction.MarkerProperty, out var source)
            ? new JsFunction(source.GetString()!)
            : null;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, JsFunction value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(JsFunction.MarkerProperty, value.Source);
        writer.WriteEndObject();
    }
}
