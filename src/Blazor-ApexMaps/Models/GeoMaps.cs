namespace Blazor_ApexMaps.Models;

/// <summary>
/// Ids for the geometry packs that ship with apexmaps, so a map id is discoverable and a typo is
/// a compile error rather than a blank map.
/// </summary>
/// <remarks>
/// <para>
/// Ids are <c>namespace/name@detail</c>. Dropping the detail suffix (<c>"world/countries"</c>)
/// asks for the lightest pack at that level, which is what a dashboard usually wants. The short
/// aliases here are the country's own word for its first-order tier, because "admin-1" is not
/// the same tier everywhere: Natural Earth's admin-1 for the UK is 232 districts and for France
/// 101 departments. A pack's <c>levelName</c> metadata says what one of its features is.
/// </para>
/// <para>
/// Nothing is fetched until a pack is named, one request per pack however many maps ask for it.
/// Each pack carries a recommended join key, a recommended projection where the generic default
/// would be wrong, and its provenance.
/// </para>
/// </remarks>
public static class GeoMaps
{
    /// <summary>World countries, 110m: 177 features. The lightest world map.</summary>
    public const string WorldCountries110m = "world/countries@110m";

    /// <summary>World countries, 50m: 242 features, including small states.</summary>
    public const string WorldCountries50m = "world/countries@50m";

    /// <summary>World countries at the lightest available detail.</summary>
    public const string WorldCountries = "world/countries";

    /// <summary>Coastline only, 110m: land masses with no internal boundaries.</summary>
    public const string WorldLand110m = "world/land@110m";

    /// <summary>Coastline only, 50m.</summary>
    public const string WorldLand50m = "world/land@50m";

    /// <summary>The world, at the lightest available detail.</summary>
    public const string World = "world";

    /// <summary>US states and territories, 56 features. Joins on <c>abbr</c> (<c>"CA"</c>).</summary>
    public const string UsStates = "us/states@10m";

    /// <summary>US states, by the short alias. Comes with <c>albersUsa</c> as its projection.</summary>
    public const string Us = "us";

    /// <summary>All 3,231 US counties. Joins on 5-digit <c>fips</c>.</summary>
    public const string UsCounties = "us/counties@10m";

    /// <summary>The US as one outline.</summary>
    public const string UsNation = "us/nation@10m";

    /// <summary>NUTS 0: EU member states, Eurostat geometry. Joins on <c>nuts_id</c>.</summary>
    public const string EuNuts0 = "eu/nuts0@20m";

    /// <summary>NUTS 1: 125 major regions.</summary>
    public const string EuNuts1 = "eu/nuts1@20m";

    /// <summary>NUTS 2: 334 basic regions. The usual choice for a European regional map.</summary>
    public const string EuNuts2 = "eu/nuts2@20m";

    /// <summary>NUTS 3: 1,514 small regions.</summary>
    public const string EuNuts3 = "eu/nuts3@20m";

    /// <summary>Australian states and territories. Joins on ISO 3166-2.</summary>
    public const string AustraliaStates = "au/states";

    /// <summary>Brazilian states.</summary>
    public const string BrazilStates = "br/states";

    /// <summary>Canadian provinces and territories.</summary>
    public const string CanadaProvinces = "ca/provinces";

    /// <summary>Chinese provinces.</summary>
    public const string ChinaProvinces = "cn/provinces";

    /// <summary>German states (Länder).</summary>
    public const string GermanyStates = "de/states";

    /// <summary>Spanish provinces.</summary>
    public const string SpainProvinces = "es/provinces";

    /// <summary>French departments.</summary>
    public const string FranceDepartments = "fr/departments";

    /// <summary>UK districts.</summary>
    public const string UkDistricts = "gb/districts";

    /// <summary>Indonesian provinces.</summary>
    public const string IndonesiaProvinces = "id/provinces";

    /// <summary>Indian states and union territories.</summary>
    public const string IndiaStates = "in/states";

    /// <summary>Italian provinces.</summary>
    public const string ItalyProvinces = "it/provinces";

    /// <summary>Japanese prefectures, all 47. Joins on ISO 3166-2 (<c>"JP-13"</c>).</summary>
    public const string JapanPrefectures = "jp/prefectures";

    /// <summary>South Korean provinces.</summary>
    public const string KoreaProvinces = "kr/provinces";

    /// <summary>Mexican states.</summary>
    public const string MexicoStates = "mx/states";

    /// <summary>Russian regions.</summary>
    public const string RussiaRegions = "ru/regions";
}

/// <summary>
/// The 13 built-in projections and their aliases. Anything registered at runtime with
/// <c>ApexMaps.registerProjection</c> is named by its own string.
/// </summary>
public static class Projections
{
    /// <summary>
    /// Equal Earth: equal-area, and the default. Mercator exaggerates high-latitude area by an
    /// order of magnitude, which is exactly wrong when area carries a value.
    /// </summary>
    public const string EqualEarth = "equalEarth";

    /// <summary>Mercator: conformal, familiar, and area-distorting.</summary>
    public const string Mercator = "mercator";

    /// <summary>Web Mercator, as used by tile services.</summary>
    public const string WebMercator = "webMercator";

    /// <summary>Web Mercator, by its EPSG code.</summary>
    public const string Epsg3857 = "epsg:3857";

    /// <summary>Equirectangular: longitude and latitude straight onto x and y.</summary>
    public const string Equirectangular = "equirectangular";

    /// <summary>Plate carrée. A synonym for <see cref="Equirectangular"/>.</summary>
    public const string PlateCarree = "plateCarree";

    /// <summary>Equirectangular, by its EPSG code.</summary>
    public const string Epsg4326 = "epsg:4326";

    /// <summary>Natural Earth: a compromise projection for world maps.</summary>
    public const string NaturalEarth = "naturalEarth";

    /// <summary>Orthographic: the globe. Dragging spins the sphere rather than panning a plane.</summary>
    public const string Orthographic = "orthographic";

    /// <summary>Albers equal-area conic. Takes <c>Parallels</c>.</summary>
    public const string Albers = "albers";

    /// <summary>Albers USA: the composite that insets Alaska and Hawaii.</summary>
    public const string AlbersUsa = "albersUsa";

    /// <summary>Conic conformal (Lambert).</summary>
    public const string ConicConformal = "conicConformal";

    /// <summary>Conic equal-area.</summary>
    public const string ConicEqualArea = "conicEqualArea";

    /// <summary>Conic equidistant.</summary>
    public const string ConicEquidistant = "conicEquidistant";

    /// <summary>Azimuthal equal-area.</summary>
    public const string AzimuthalEqualArea = "azimuthalEqualArea";

    /// <summary>Azimuthal equidistant: distances from the centre are true.</summary>
    public const string AzimuthalEquidistant = "azimuthalEquidistant";

    /// <summary>Gnomonic.</summary>
    public const string Gnomonic = "gnomonic";

    /// <summary>Stereographic.</summary>
    public const string Stereographic = "stereographic";

    /// <summary>Transverse Mercator.</summary>
    public const string TransverseMercator = "transverseMercator";

    /// <summary>
    /// Identity: coordinates straight through, for geometry that is not geographic at all
    /// (a floor plan, a seating chart, a board).
    /// </summary>
    public const string Identity = "identity";
}

/// <summary>
/// The 17 built-in palettes. Ramps are sampled in OkLab, because interpolating through sRGB
/// turns midpoints muddy grey and readers infer magnitude from perceived lightness.
/// </summary>
public static class Palettes
{
    /// <summary>Sequential blues.</summary>
    public const string Blues = "blues";

    /// <summary>Sequential greens.</summary>
    public const string Greens = "greens";

    /// <summary>Sequential oranges.</summary>
    public const string Oranges = "oranges";

    /// <summary>Sequential reds.</summary>
    public const string Reds = "reds";

    /// <summary>Sequential purples.</summary>
    public const string Purples = "purples";

    /// <summary>Sequential greys.</summary>
    public const string Greys = "greys";

    /// <summary>Viridis: perceptually uniform, colour-vision safe.</summary>
    public const string Viridis = "viridis";

    /// <summary>Magma: perceptually uniform.</summary>
    public const string Magma = "magma";

    /// <summary>Sequential teal.</summary>
    public const string Teal = "teal";

    /// <summary>Diverging red to blue.</summary>
    public const string RdBu = "rdbu";

    /// <summary>Diverging brown to blue-green.</summary>
    public const string BrBg = "brbg";

    /// <summary>Diverging pink to yellow-green.</summary>
    public const string PiYg = "piyg";

    /// <summary>Diverging spectral.</summary>
    public const string Spectral = "spectral";

    /// <summary>Diverging red-yellow-green.</summary>
    public const string RdYlGn = "rdylgn";

    /// <summary>The ApexCharts categorical palette.</summary>
    public const string Apex = "apex";

    /// <summary>The Tableau 10 categorical palette.</summary>
    public const string Tableau = "tableau";

    /// <summary>
    /// Okabe-Ito: a categorical palette distinguishable with every common form of colour vision
    /// deficiency. Free in every tier, permanently.
    /// </summary>
    public const string OkabeIto = "okabeIto";
}
