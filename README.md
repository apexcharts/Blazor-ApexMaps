# Blazor-ApexMaps

A Blazor wrapper for [ApexMaps](https://github.com/apexcharts/apexmaps): interactive geographic data
visualization, with the whole option surface typed in C#.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/Blazor-ApexMaps.svg)](https://www.nuget.org/packages/Blazor-ApexMaps/)
[![Downloads](https://img.shields.io/nuget/dt/Blazor-ApexMaps.svg)](https://www.nuget.org/packages/Blazor-ApexMaps/)

```razor
<ApexMaps Options="_options" Height="440" />

@code {
    private readonly MapsOptions _options = new()
    {
        Geo = new GeoOptions { Map = GeoMaps.WorldCountries110m },
        Series =
        [
            new ChoroplethSeries
            {
                Name = "Unemployment rate",
                JoinBy = JoinSpec.Between("iso_a3", "code"),
                Data = [new { code = "FRA", value = 7.3 }, new { code = "DEU", value = 5.7 }],
            },
        ],
    };
}
```

That is the whole example. No geometry to find, host or parse, no script tag, no CDN reference, no
stylesheet link, and no projection, palette, classification, legend, label or tooltip
configuration: the defaults are meant to be publishable.

**[Live demos](https://apexcharts.github.io/Blazor-ApexMaps/)** - one page per feature.

## What you get

| Area | Detail |
| --- | --- |
| Series | `ChoroplethSeries`, `BubbleSeries` (proportional symbols), `MarkerSeries` (seven shapes, categorical colour, clustering †), `ArcSeries` (great circles with travelling flow beads) †, `LineSeries` (routes through given vertices) †, plus an automatic basemap when no feature series is present |
| Geometry | 26 built-in packs named through `GeoMaps`: world countries and coastline, US states and all 3,231 counties, EU NUTS 0-3, and admin-1 for 15 more countries. Lazy, one request per pack, provenance and attribution attached |
| Projections | 13 projections with aliases through `Projections`, spec objects with rotation, centre, standard parallels and clip angle, and a globe that spins under the drag |
| Data | Your own models, records, anonymous types or dictionaries. Join-key auto-detection, mismatch diagnostics with suggestions through `DiagnoseJoinAsync`, opt-in fuzzy matching |
| Scales | Quantile, equal interval, Jenks, threshold, linear, log, sqrt, ordinal; 17 palettes sampled in OkLab; square-root size scales with nested-circle legends |
| Fills | Flat colour, eight pattern tiles with automatic ink contrast †, or an image per region clipped to its own outline † |
| Interaction | Anchored wheel zoom, inertial pan, pinch, on-screen controls, globe dragging, hover states, click and box selection, cross-map linked selection †, legend class muting, drilldown with a breadcrumb † |
| Camera | `FlyToAsync`, `EaseToAsync`, `JumpToAsync`, `FrameFeatureAsync`, `ResetViewAsync`, `RotateToAsync`, all interruptible; on a globe a move to a place turns the sphere |
| Components | Classed, gradient and nested-circle legends with a hover marker, HTML tooltips, collision-avoiding labels, editorial annotations † |
| Events | All 17 core events as `EventCallback` parameters, with typed payloads |
| Export | `ExportPngAsync`, `ExportSvgAsync`, `GetDataUriAsync`, `GetSvgStringAsync`, and `GetSpecAsync` for the resolved options tree |
| Accessibility | ARIA roles, an auto-generated description, roving-tabindex keyboard navigation, an optional data table, `prefers-reduced-motion`. On by default and free in every tier |
| Platform | .NET 9, nullable reference types, XML docs on every option, Blazor WebAssembly and Blazor Server |

† Licensed feature. It works without a key so you can evaluate it, with a watermark on the map. See
[Licensing](#licensing).

## Installation

```bash
dotnet add package Blazor-ApexMaps
```

```csharp
// Program.cs
using Blazor_ApexMaps.Extensions;

builder.Services.AddApexMaps();
```

```razor
@* _Imports.razor *@
@using Blazor_ApexMaps.Components
@using Blazor_ApexMaps.Models
```

That is all the setup there is. The package carries the apexmaps core as an ES module and imports it
itself, and the core injects its own stylesheet, so there is nothing to add to `index.html` or
`App.razor`.

## Geometry

Finding, converting and hosting boundaries is the tax on every map project, and it is paid before
any chart is drawn, so the geometry is part of the product:

```csharp
Geo = new GeoOptions { Map = GeoMaps.WorldCountries110m }  // canonical id
Geo = new GeoOptions { Map = GeoMaps.WorldCountries }      // detail-free: the lightest one
Geo = new GeoOptions { Map = GeoMaps.Us }                  // states, with albersUsa
Geo = new GeoOptions { Map = GeoMaps.JapanPrefectures }    // the country's own word for its tier
Geo = new GeoOptions { Map = GeoMaps.EuNuts2 }             // Eurostat regions
Geo = new GeoOptions { Map = "https://example.com/districts.json" }        // your own URL
Geo = new GeoOptions { Map = MapSource.FromGeometry(myGeoJsonObject) }     // geometry in hand
```

Nothing is fetched until a pack is named, and one pack is one request however many maps on the page
ask for it. Each pack carries a recommended join key, a recommended projection where the generic
default would be wrong, and its provenance.

Packs come from the `apexmaps-geo` package on jsDelivr by default, so nothing needs installing. For
an offline or air-gapped app, publish that package's files under `wwwroot` and point the library at
them:

```csharp
builder.Services.AddApexMaps(options =>
{
    options.GeoSource = "/apexmaps-geo/";
});
```

`ApexMapsRuntimeService` reaches the rest of the registry at runtime: `RegisterMapAsync` (any
geometry under an id of your own, including a floor plan drawn against the `identity` projection),
`RegisterPaletteAsync`, `ListMapsAsync`, `ListProjectionsAsync`, `ListPalettesAsync` and
`MapMetaAsync` for a pack's source, licence, attribution and vintage.

## The component

```razor
<ApexMaps @ref="_map"
          Options="_options"
          Series="_series"
          Map="@GeoMaps.UsStates"
          Height="480"
          Width="@("100%")"
          Class="my-map"
          AutoUpdate="true"
          OnFeatureClick="OnFeatureClick"
          OnSelectionChange="OnSelectionChange" />
```

`Options` is the whole tree; `Series`, `Map`, `Width` and `Height` are shorthands for the parts of
it that change most often. With `AutoUpdate` on (the default), a parameter change is compared
against what was last sent to the browser and applied the cheap way where it can be: a change
confined to the series tweens fills and radii rather than rebuilding the DOM, and a changed map or
projection reprojects. A parent that rebuilds its options object on every render is not a redraw.

### Methods

Take a `@ref` and drive the map directly:

```csharp
await _map.UpdateSeriesAsync(nextSeries);                              // tweens
await _map.UpdateOptionsAsync(new MapsOptions { Geo = new GeoOptions { Projection = Projections.Mercator } });
await _map.FlyToAsync(new CameraTarget { Center = new LonLat(2.35, 48.85), Zoom = 8 });
await _map.FrameFeatureAsync("FRA", new FrameOptions { Padding = 40 });
await _map.ResetViewAsync();
await _map.ZoomInAsync(); await _map.ZoomOutAsync();
await _map.RotateToAsync(-25, -18);                                    // turns a globe
await _map.DrillToAsync("CA"); await _map.DrillUpAsync(int.MaxValue);
await _map.SetSelectionAsync("FRA", "DEU"); await _map.ClearSelectionAsync();
var diagnostics = await _map.DiagnoseJoinAsync();
var spec = await _map.GetSpecAsync();                                  // resolved options, as JSON
await _map.ExportPngAsync(new MapExportOptions { Scale = 2 });
var uri = await _map.GetDataUriAsync();
```

### Events

All 17, as `EventCallback` parameters: `OnRendered`, `OnUpdated`, `OnResized`, `OnFeatureClick`,
`OnFeatureHover`, `OnFeatureFocus`, `OnMarkClick`, `OnMarkHover`, `OnClusterClick`, `OnDrilldown`,
`OnDrillup`, `OnSelectionChange`, `OnLegendToggle`, `OnZoom`, `OnPanEnd`, `OnRotate`, `OnRotateEnd`.

```csharp
private void OnFeatureClick(FeatureEventArgs e)
{
    _selected = e.Key;                    // join key
    _label = e.Name;                      // display name
    _value = e.Value;                     // null where the feature had no data
    var row = e.Read<CountryRow>();       // the data row, as your own model
    var iso = e.Property("iso_a3");       // a geometry property
}
```

### Formatters and accessors

The options that take a function in the core take one here too, as JavaScript source through
`JsFunction`. They run per mark, per frame, or during projection, so a round trip to .NET for each
of a few thousand features would be a stutter rather than a chart: the source travels as a string
and is compiled once, in the browser.

```csharp
Tooltip = new TooltipOptions
{
    Formatter = JsFunction.From("ctx => `<b>${ctx.name}</b>: ${ctx.value ?? 'no data'}`"),
},
Fill = new SeriesFillOptions
{
    Pattern = JsFunction.From("ctx => ctx.value > 55 ? ({ type: 'crosshatch' }) : null"),
},
```

A `JsFunction` is compiled with `new Function`, so it runs in the global scope: it can see `window`
and anything your app put there, and it cannot see your C# variables. It is evaluated as code, so
never build one from untrusted input.

Anything this wrapper does not model yet goes through `AdditionalOptions`, on `MapsOptions` and on
every series, which is merged in with its keys written exactly as given.

## Licensing

Dual licensed on the same terms as the rest of the family: a free **Community License** for
individuals, non-profits, educators and organizations under $2M USD annual revenue, and a paid
Commercial or OEM license above that. One key works across every Apex product, so an ApexCharts or
ApexGrid customer does not buy a second one for maps. See [LICENSE](LICENSE).

The line is that **a map that answers a question is free, and a map that becomes an application is
licensed.**

| Free, always | Licensed |
| --- | --- |
| Choropleth, bubble and marker series, and the automatic basemap | Point clustering (`Cluster`) |
| Every one of the 13 built-in projections, with spec objects | Projections you register yourself |
| The geometry registry, all 26 packs, provenance and attribution | Drilldown and the breadcrumb (`Drilldown`) |
| Tooltips, legends, labels, data labels, states and themes | Editorial annotations (`Annotations`) |
| Zoom, pan, pinch, hover, click and box selection, the camera API | Arc and line route series |
| Joins, fuzzy matching, and the join diagnostics | Linked selection across maps (`Link`) |
| Scales, palettes, size legends, responsive rules | Story mode (`ChartContext.Story`) |
| Flat fills, in every scale and palette | Pattern fills and image fills |
| PNG and SVG export | |
| The accessibility layer | |

Without a valid key the licensed features **still work, in full, with a watermark on the map**, so
you can evaluate them in your own app with your own data. A valid key removes the watermark, without
a reload. A map that uses none of them renders clean, with or without a key.

```csharp
builder.Services.AddApexMaps(options =>
{
    options.LicenseKey = "APEX-xxxxxxxx";
});
```

Keys are per product, not per page: `AddApexMaps` and `AddApexCharts` set different copies of the
licence manager, because each library bundles its own. An app with a chart and a map configures
both, with the same key. Get a key at [apexcharts.com/pricing](https://apexcharts.com/pricing).

**The software licence does not cover the geographic data**, which is published by third parties
under its own terms: Natural Earth and US Census TIGER/Line are public domain, Eurostat GISCO NUTS
is CC BY 4.0 and is credited on screen automatically. `MapMetaAsync(id)` says which is which.

## Requirements

- .NET 9.0 or later
- Blazor WebAssembly or Blazor Server
- A browser with ES module support (every current one)

## The demo app

```bash
git clone https://github.com/apexcharts/Blazor-ApexMaps.git
cd Blazor-ApexMaps
dotnet run --project src/Blazor-ApexMaps.Sample
```

One page per feature, so each loads on its own and cannot be broken by an unrelated one: choropleth,
scales and palettes, projections and the globe, bubbles and markers, arcs and routes, drilldown,
selection and linking, camera and export, theming and annotations, join diagnostics, and every event
as it happens.

## Development

```bash
dotnet build Blazor-ApexMaps.sln
dotnet pack src/Blazor-ApexMaps/Blazor-ApexMaps.csproj -c Release

# End-to-end: drive the sample in a real browser and fail on a page that draws nothing
dotnet run --project src/Blazor-ApexMaps.Sample --urls http://localhost:5186 &
cd tests/e2e && npm install && npx playwright install --with-deps chromium && npm test
```

The e2e smoke test is the one that matters for a wrapper: the whole contract lives in the interop
layer and the core, so a mis-serialized option, a broken import path, an uncompilable formatter or a
moved pack id would only surface at runtime. It loads every demo page in headless Chromium and fails
on a console error, a visible Blazor error UI, a map that drew zero marks, an event that never
reached .NET, or an export that produced nothing.

### Project structure

```
src/Blazor-ApexMaps/
  Components/ApexMaps.razor        the component: parameters, methods, event bridge
  Models/                          the option tree, series, data rows, events, enums
  Interop/ApexMapsInterop.cs       the .NET side of the bridge
  Serialization/                   camelCase JSON, and JsFunction
  Services/ApexMapsRuntimeService  licence, geometry source, the registry
  wwwroot/js/blazor-apexmaps.js    the JS side of the bridge
  wwwroot/js/apexmaps.esm.js       the vendored apexmaps core
src/Blazor-ApexMaps.Sample/        the demo app, one page per feature
tests/e2e/                         headless browser smoke and interaction test
```

## Related

- [ApexMaps](https://github.com/apexcharts/apexmaps) - the core library, with `react-apexmaps`,
  `vue-apexmaps` and `ngx-apexmaps` alongside it
- [Blazor-ApexTree](https://github.com/apexcharts/Blazor-ApexTree),
  [Blazor-ApexSankey](https://github.com/apexcharts/Blazor-ApexSankey),
  [Blazor-ApexGantt](https://github.com/apexcharts/Blazor-ApexGantt) - the rest of the family
- [ApexCharts](https://apexcharts.com)

## Support

- Issues: [github.com/apexcharts/Blazor-ApexMaps/issues](https://github.com/apexcharts/Blazor-ApexMaps/issues)
- Licensing: [apexcharts.com/pricing](https://apexcharts.com/pricing)
