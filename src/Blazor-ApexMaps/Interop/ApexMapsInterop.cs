using Blazor_ApexMaps.Models;
using Microsoft.JSInterop;

namespace Blazor_ApexMaps.Interop;

/// <summary>
/// The JavaScript side of the wrapper. Loads the interop bridge as an ES module, which imports the
/// vendored apexmaps core, so a host app needs no script tag, no CDN reference and no stylesheet
/// link: the core injects its own CSS.
/// </summary>
/// <remarks>
/// Registered as scoped, so one instance serves a circuit or a WebAssembly app and the module is
/// imported once. The <c>ApexMaps</c> component creates its own for the map's lifetime.
/// </remarks>
public class ApexMapsInterop : IAsyncDisposable
{
    private const string ModulePath = "./_content/Blazor-ApexMaps/js/blazor-apexmaps.js?ver=0.3.0";

    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;

    /// <summary>Creates the interop layer over a JS runtime.</summary>
    public ApexMapsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task<IJSObjectReference> GetModuleAsync()
        => _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);

    // --- statics -------------------------------------------------------------

    /// <summary>Sets the licence key. One key covers every Apex product, but each needs its own call.</summary>
    public async Task<bool> SetLicenseAsync(string licenseKey)
    {
        if (string.IsNullOrWhiteSpace(licenseKey)) return false;
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("setLicense", licenseKey);
    }

    /// <summary>Points the geometry registry at a base URL: a self-hosted copy, or your own CDN.</summary>
    public async Task<bool> SetGeoSourceAsync(string source)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("setGeoSource", source);
    }

    /// <summary>Registers geometry under an id, so a map can name it like a built-in pack.</summary>
    public async Task<bool> RegisterMapAsync(string id, string geometryJson, string? metaJson = null)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("registerMap", id, geometryJson, metaJson);
    }

    /// <summary>Registers a named palette.</summary>
    public async Task<bool> RegisterPaletteAsync(string name, string paletteJson)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("registerPalette", name, paletteJson);
    }

    /// <summary>Every registered map id, built-ins and aliases included.</summary>
    public async Task<string[]> ListMapsAsync()
        => await (await GetModuleAsync()).InvokeAsync<string[]>("listMaps");

    /// <summary>Every registered projection name.</summary>
    public async Task<string[]> ListProjectionsAsync()
        => await (await GetModuleAsync()).InvokeAsync<string[]>("listProjections");

    /// <summary>Every registered palette name.</summary>
    public async Task<string[]> ListPalettesAsync()
        => await (await GetModuleAsync()).InvokeAsync<string[]>("listPalettes");

    /// <summary>Provenance and recommendations for a geometry pack.</summary>
    public async Task<MapMeta?> MapMetaAsync(string id)
        => await (await GetModuleAsync()).InvokeAsync<MapMeta?>("mapMeta", id);

    /// <summary>The core library's version.</summary>
    public async Task<string> VersionAsync()
        => await (await GetModuleAsync()).InvokeAsync<string>("version");

    // --- lifecycle -----------------------------------------------------------

    /// <summary>
    /// Creates a map in an element and renders it. Awaits the geometry, so it returns with the map
    /// on screen.
    /// </summary>
    public async Task<bool> InitAsync<T>(string elementId, string optionsJson, DotNetObjectReference<T>? dotNetRef = null)
        where T : class
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("init", elementId, optionsJson, dotNetRef);
    }

    /// <summary>Merges options into a live map and redraws.</summary>
    public async Task UpdateOptionsAsync(string elementId, string optionsJson, bool redrawGeometry = false)
        => await (await GetModuleAsync()).InvokeVoidAsync("updateOptions", elementId, optionsJson, redrawGeometry);

    /// <summary>Replaces the series on a live map.</summary>
    public async Task UpdateSeriesAsync(string elementId, string seriesJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("updateSeries", elementId, seriesJson);

    /// <summary>Destroys a map and unsubscribes its events.</summary>
    public async Task DestroyAsync(string elementId)
    {
        if (_module is null) return;
        await _module.InvokeVoidAsync("destroy", elementId);
    }

    // --- drilldown -----------------------------------------------------------

    /// <summary>Drills into a feature by key, as a click would.</summary>
    public async Task<bool> DrillToAsync(string elementId, string key)
        => await (await GetModuleAsync()).InvokeAsync<bool>("drillTo", elementId, key);

    /// <summary>Climbs back out. Pass a large number to return to the top.</summary>
    public async Task<bool> DrillUpAsync(string elementId, int levels)
        => await (await GetModuleAsync()).InvokeAsync<bool>("drillUp", elementId, levels);

    /// <summary>How deep the map currently is. 0 is the top.</summary>
    public async Task<int> DrillDepthAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<int>("drillDepth", elementId);

    // --- selection -----------------------------------------------------------

    /// <summary>Selects a feature, or deselects it when it is already selected.</summary>
    public async Task ToggleSelectionAsync(string elementId, string key)
        => await (await GetModuleAsync()).InvokeVoidAsync("toggleSelection", elementId, key);

    /// <summary>Replaces the selection.</summary>
    public async Task SetSelectionAsync(string elementId, string keysJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("setSelection", elementId, keysJson);

    /// <summary>Clears the selection.</summary>
    public async Task ClearSelectionAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearSelection", elementId);

    // --- camera --------------------------------------------------------------

    /// <summary>Flies along a zoom-and-pan path, or turns the sphere on a globe.</summary>
    public async Task FlyToAsync(string elementId, string targetJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("flyTo", elementId, targetJson);

    /// <summary>Moves with a fixed duration and easing.</summary>
    public async Task EaseToAsync(string elementId, string targetJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("easeTo", elementId, targetJson);

    /// <summary>Moves with no animation.</summary>
    public async Task JumpToAsync(string elementId, string targetJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("jumpTo", elementId, targetJson);

    /// <summary>Frames one feature by key.</summary>
    public async Task FrameFeatureAsync(string elementId, string key, string? optionsJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("frameFeature", elementId, key, optionsJson);

    /// <summary>Returns to the opening fit, and a spun globe to the rotation it opened at.</summary>
    public async Task ResetViewAsync(string elementId, string? optionsJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("resetView", elementId, optionsJson);

    /// <summary>Steps the zoom in, as the <c>+</c> control does.</summary>
    public async Task ZoomInAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("zoomIn", elementId);

    /// <summary>Steps the zoom out.</summary>
    public async Task ZoomOutAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("zoomOut", elementId);

    /// <summary>The camera's current scale, 1 at the opening fit.</summary>
    public async Task<double> GetZoomAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<double>("getZoom", elementId);

    /// <summary>Turns the globe to an absolute rotation, as a drag would.</summary>
    public async Task RotateToAsync(string elementId, string anglesJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("rotateTo", elementId, anglesJson);

    /// <summary>The projection's rotation, <c>[lambda, phi, gamma]</c> in degrees.</summary>
    public async Task<double[]> GetRotationAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<double[]>("getRotation", elementId);

    // --- diagnostics and export ---------------------------------------------

    /// <summary>What a series' join actually matched, and what it did not.</summary>
    public async Task<JoinDiagnostics?> DiagnoseJoinAsync(string elementId, int seriesIndex)
        => await (await GetModuleAsync()).InvokeAsync<JoinDiagnostics?>("diagnoseJoin", elementId, seriesIndex);

    /// <summary>The resolved options tree as JSON, with functions dropped.</summary>
    public async Task<string> ToSpecAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<string>("toSpec", elementId);

    /// <summary>The current view as a standalone SVG document.</summary>
    public async Task<string> GetSvgStringAsync(string elementId, string? optionsJson)
        => await (await GetModuleAsync()).InvokeAsync<string>("getSvgString", elementId, optionsJson);

    /// <summary>Downloads the current view as an SVG file.</summary>
    public async Task ExportSvgAsync(string elementId, string? optionsJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("exportSVG", elementId, optionsJson);

    /// <summary>Downloads the current view as a PNG file.</summary>
    public async Task ExportPngAsync(string elementId, string? optionsJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("exportPNG", elementId, optionsJson);

    /// <summary>The current view as a PNG data URI, for embedding rather than downloading.</summary>
    public async Task<string> DataUriAsync(string elementId, string? optionsJson)
        => await (await GetModuleAsync()).InvokeAsync<string>("dataURI", elementId, optionsJson);

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); }
            catch (JSDisconnectedException) { /* circuit already gone */ }
        }
        GC.SuppressFinalize(this);
    }
}
