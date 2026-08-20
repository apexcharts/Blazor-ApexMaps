using Blazor_ApexMaps.Configuration;
using Blazor_ApexMaps.Interop;
using Blazor_ApexMaps.Models;
using Blazor_ApexMaps.Serialization;
using Microsoft.Extensions.Options;

namespace Blazor_ApexMaps.Services;

/// <summary>
/// The library-wide settings and the registry, as a service: the licence key, the geometry source,
/// and the calls that register geometry, palettes and query the catalogue.
/// </summary>
/// <remarks>
/// The one-time settings are applied on the first map to render, and once only however many maps a
/// page holds. Everything else here is a static on the core library, so it applies to every map on
/// the page rather than to one of them.
/// </remarks>
public class ApexMapsRuntimeService
{
    private readonly ApexMapsInterop _interop;
    private readonly ApexMapsConfiguration _configuration;
    private bool _initialized;

    /// <summary>Creates the service. Resolved from the container; you do not construct it.</summary>
    public ApexMapsRuntimeService(ApexMapsInterop interop, IOptions<ApexMapsConfiguration> configuration)
    {
        _interop = interop;
        _configuration = configuration.Value;
    }

    /// <summary>
    /// Applies the configured licence key and geometry source. Called by the component before it
    /// creates a map, and a no-op after the first time.
    /// </summary>
    public async Task<bool> InitializeAsync()
    {
        if (_initialized) return true;
        _initialized = true;

        if (!string.IsNullOrWhiteSpace(_configuration.GeoSource))
        {
            await _interop.SetGeoSourceAsync(_configuration.GeoSource);
        }

        if (!string.IsNullOrWhiteSpace(_configuration.LicenseKey))
        {
            return await _interop.SetLicenseAsync(_configuration.LicenseKey);
        }

        // No key configured: the licensed features still work, watermarked.
        return true;
    }

    /// <summary>Sets the licence key at runtime. Removes the watermark without a reload.</summary>
    public Task<bool> SetLicenseAsync(string licenseKey) => _interop.SetLicenseAsync(licenseKey);

    /// <summary>Points the geometry registry at a base URL.</summary>
    public Task<bool> SetGeoSourceAsync(string source) => _interop.SetGeoSourceAsync(source);

    /// <summary>
    /// Registers geometry under an id, so <see cref="GeoOptions.Map"/> can name it like a built-in
    /// pack. Geometry can be anything that serializes to GeoJSON or TopoJSON, including a floor
    /// plan or a seating chart drawn against the <c>identity</c> projection.
    /// </summary>
    public Task<bool> RegisterMapAsync(string id, object geometry, MapMeta? meta = null)
        => _interop.RegisterMapAsync(
            id,
            MapsSerializer.Serialize(geometry),
            meta is null ? null : MapsSerializer.Serialize(meta));

    /// <summary>
    /// Registers a palette: <c>kind</c> is <c>"sequential"</c>, <c>"diverging"</c> or
    /// <c>"categorical"</c>, and <c>stops</c> is the colour list it is sampled from.
    /// </summary>
    public Task<bool> RegisterPaletteAsync(string name, string kind, IEnumerable<string> stops)
        => _interop.RegisterPaletteAsync(
            name,
            MapsSerializer.Serialize(new Dictionary<string, object> { ["kind"] = kind, ["stops"] = stops }));

    /// <summary>Every registered map id, built-in packs and aliases included.</summary>
    public Task<string[]> ListMapsAsync() => _interop.ListMapsAsync();

    /// <summary>Every registered projection name.</summary>
    public Task<string[]> ListProjectionsAsync() => _interop.ListProjectionsAsync();

    /// <summary>Every registered palette name.</summary>
    public Task<string[]> ListPalettesAsync() => _interop.ListPalettesAsync();

    /// <summary>
    /// Provenance for a pack: its source, licence, attribution, vintage, recommended join key and
    /// boundary policy. Required attribution is rendered by the map on its own.
    /// </summary>
    public Task<MapMeta?> MapMetaAsync(string id) => _interop.MapMetaAsync(id);

    /// <summary>The core library's version.</summary>
    public Task<string> VersionAsync() => _interop.VersionAsync();
}
