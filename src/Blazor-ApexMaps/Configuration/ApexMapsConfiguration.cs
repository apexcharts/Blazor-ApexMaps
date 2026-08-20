namespace Blazor_ApexMaps.Configuration;

/// <summary>
/// App-wide ApexMaps settings, applied once before the first map renders.
/// </summary>
public class ApexMapsConfiguration
{
    /// <summary>
    /// Licence key. One key covers every Apex product, but each library holds its own copy of the
    /// licence manager, so a page with a chart and a map sets both with the same key.
    /// </summary>
    /// <remarks>
    /// Without a key the licensed features still work, in full, with a watermark on the map, so
    /// they can be evaluated in your own app with your own data. A map that uses none of them
    /// renders clean either way. Get a key at https://apexcharts.com/pricing.
    /// </remarks>
    public string? LicenseKey { get; set; }

    /// <summary>
    /// Base URL the geometry packs are fetched from. Defaults to the <c>apexmaps-geo</c> package on
    /// jsDelivr, so nothing needs installing or hosting.
    /// </summary>
    /// <remarks>
    /// Point this at your own copy for an air-gapped or offline app: publish the contents of the
    /// <c>apexmaps-geo</c> package under <c>wwwroot</c> and set, for example,
    /// <c>"/apexmaps-geo/"</c>. Packs are still fetched lazily, one request per pack.
    /// </remarks>
    public string? GeoSource { get; set; }
}
