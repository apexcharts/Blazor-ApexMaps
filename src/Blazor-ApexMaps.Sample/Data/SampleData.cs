using Blazor_ApexMaps.Models;

namespace Blazor_ApexMaps.Sample.Data;

/// <summary>A country row, keyed by ISO 3166-1 alpha-3.</summary>
/// <param name="Code">ISO alpha-3 code, which is what <c>world/countries</c> joins on.</param>
/// <param name="Name">Country name, for tooltips.</param>
/// <param name="Value">The value the choropleth colours by.</param>
/// <param name="Population">Population, for the normalised view.</param>
public record CountryRow(string Code, string Name, double Value, double Population);

/// <summary>A US state row, keyed by its postal abbreviation.</summary>
/// <param name="Abbr">Two-letter abbreviation, which is what <c>us/states</c> joins on.</param>
/// <param name="Name">State name.</param>
/// <param name="Value">The value the choropleth colours by.</param>
public record StateRow(string Abbr, string Name, double Value);

/// <summary>
/// Data for the demos. Illustrative figures, not a published dataset: enough rows to show what a
/// scale, a join or a legend does, and small enough to read in the source.
/// </summary>
public static class SampleData
{
    /// <summary>Renewable share of electricity generation, percent, for a spread of countries.</summary>
    public static readonly CountryRow[] Renewables =
    [
        new("NOR", "Norway", 98.5, 5.4),
        new("BRA", "Brazil", 83.0, 214.3),
        new("CAN", "Canada", 67.9, 38.9),
        new("SWE", "Sweden", 67.5, 10.5),
        new("NZL", "New Zealand", 62.5, 5.1),
        new("DNK", "Denmark", 62.0, 5.9),
        new("PRT", "Portugal", 58.4, 10.3),
        new("ESP", "Spain", 47.1, 47.6),
        new("DEU", "Germany", 44.0, 83.2),
        new("GBR", "United Kingdom", 41.4, 67.3),
        new("ITA", "Italy", 41.0, 59.1),
        new("CHL", "Chile", 39.8, 19.5),
        new("FRA", "France", 25.1, 67.8),
        new("CHN", "China", 29.6, 1412.0),
        new("IND", "India", 21.4, 1408.0),
        new("USA", "United States of America", 21.3, 333.3),
        new("JPN", "Japan", 22.0, 125.5),
        new("AUS", "Australia", 32.5, 25.7),
        new("MEX", "Mexico", 19.1, 126.7),
        new("ZAF", "South Africa", 9.6, 59.9),
        new("RUS", "Russia", 18.7, 143.4),
        new("SAU", "Saudi Arabia", 0.4, 35.9),
        new("KOR", "South Korea", 7.5, 51.7),
        new("IDN", "Indonesia", 19.2, 275.5),
        new("NGA", "Nigeria", 18.9, 213.4),
        new("EGY", "Egypt", 11.6, 109.3),
        new("ARG", "Argentina", 30.2, 45.8),
        new("TUR", "Turkey", 42.3, 84.8),
        new("POL", "Poland", 17.2, 37.7),
        new("KEN", "Kenya", 89.5, 53.0),
    ];

    /// <summary>
    /// The same countries, keyed by name rather than code, and spelled the way a spreadsheet
    /// usually spells them. Used by the join demo, where the mismatches are the point.
    /// </summary>
    public static readonly object[] RenewablesByName =
    [
        new { name = "Norway", value = 98.5 },
        new { name = "Brazil", value = 83.0 },
        new { name = "United States", value = 21.3 },
        new { name = "Russia", value = 18.7 },
        new { name = "South Korea", value = 7.5 },
        new { name = "Ivory Coast", value = 26.6 },
        new { name = "Czech Republic", value = 15.3 },
        new { name = "Germany", value = 44.0 },
        new { name = "France", value = 25.1 },
        new { name = "Kenya", value = 89.5 },
    ];

    /// <summary>Broadband adoption by US state, percent of households.</summary>
    public static readonly StateRow[] Broadband =
    [
        new("CA", "California", 89.2), new("TX", "Texas", 85.1), new("FL", "Florida", 86.4),
        new("NY", "New York", 87.6), new("PA", "Pennsylvania", 85.9), new("IL", "Illinois", 86.8),
        new("OH", "Ohio", 85.2), new("GA", "Georgia", 84.7), new("NC", "North Carolina", 84.9),
        new("MI", "Michigan", 85.6), new("NJ", "New Jersey", 90.1), new("VA", "Virginia", 88.3),
        new("WA", "Washington", 91.4), new("AZ", "Arizona", 86.0), new("MA", "Massachusetts", 90.6),
        new("TN", "Tennessee", 83.4), new("IN", "Indiana", 84.1), new("MO", "Missouri", 83.9),
        new("MD", "Maryland", 89.7), new("WI", "Wisconsin", 85.4), new("CO", "Colorado", 90.2),
        new("MN", "Minnesota", 87.9), new("SC", "South Carolina", 82.6), new("AL", "Alabama", 81.3),
        new("LA", "Louisiana", 80.4), new("KY", "Kentucky", 82.1), new("OR", "Oregon", 89.4),
        new("OK", "Oklahoma", 81.7), new("CT", "Connecticut", 89.1), new("UT", "Utah", 91.8),
        new("IA", "Iowa", 84.3), new("NV", "Nevada", 87.2), new("AR", "Arkansas", 79.8),
        new("MS", "Mississippi", 77.4), new("KS", "Kansas", 84.6), new("NM", "New Mexico", 79.1),
        new("NE", "Nebraska", 85.8), new("WV", "West Virginia", 78.2), new("ID", "Idaho", 86.7),
        new("HI", "Hawaii", 88.5), new("NH", "New Hampshire", 90.4), new("ME", "Maine", 86.1),
        new("MT", "Montana", 84.2), new("RI", "Rhode Island", 88.0), new("DE", "Delaware", 88.9),
        new("SD", "South Dakota", 84.5), new("ND", "North Dakota", 85.0), new("AK", "Alaska", 85.7),
        new("VT", "Vermont", 86.3), new("WY", "Wyoming", 84.8), new("DC", "District of Columbia", 88.1),
    ];

    /// <summary>Metropolitan populations, in millions, positioned rather than joined.</summary>
    public static readonly BubbleDatum[] MetroPopulations =
    [
        new() { Name = "Tokyo", Lon = 139.69, Lat = 35.69, Value = 37.4 },
        new() { Name = "Delhi", Lon = 77.21, Lat = 28.61, Value = 32.9 },
        new() { Name = "Shanghai", Lon = 121.47, Lat = 31.23, Value = 29.2 },
        new() { Name = "Dhaka", Lon = 90.41, Lat = 23.81, Value = 23.2 },
        new() { Name = "São Paulo", Lon = -46.63, Lat = -23.55, Value = 22.6 },
        new() { Name = "Cairo", Lon = 31.24, Lat = 30.04, Value = 22.2 },
        new() { Name = "Mexico City", Lon = -99.13, Lat = 19.43, Value = 22.1 },
        new() { Name = "Beijing", Lon = 116.41, Lat = 39.90, Value = 21.8 },
        new() { Name = "Mumbai", Lon = 72.88, Lat = 19.08, Value = 21.3 },
        new() { Name = "Osaka", Lon = 135.50, Lat = 34.69, Value = 19.0 },
        new() { Name = "New York", Lon = -74.01, Lat = 40.71, Value = 18.9 },
        new() { Name = "Karachi", Lon = 67.01, Lat = 24.86, Value = 17.2 },
        new() { Name = "Buenos Aires", Lon = -58.38, Lat = -34.60, Value = 15.4 },
        new() { Name = "Lagos", Lon = 3.38, Lat = 6.52, Value = 15.4 },
        new() { Name = "Istanbul", Lon = 28.98, Lat = 41.01, Value = 15.8 },
        new() { Name = "Moscow", Lon = 37.62, Lat = 55.76, Value = 12.7 },
        new() { Name = "Paris", Lon = 2.35, Lat = 48.86, Value = 11.2 },
        new() { Name = "London", Lon = -0.13, Lat = 51.51, Value = 9.6 },
        new() { Name = "Los Angeles", Lon = -118.24, Lat = 34.05, Value = 12.5 },
        new() { Name = "Jakarta", Lon = 106.85, Lat = -6.21, Value = 11.2 },
        new() { Name = "Lima", Lon = -77.04, Lat = -12.05, Value = 11.0 },
        new() { Name = "Johannesburg", Lon = 28.03, Lat = -26.20, Value = 6.2 },
        new() { Name = "Sydney", Lon = 151.21, Lat = -33.87, Value = 5.3 },
        new() { Name = "Toronto", Lon = -79.38, Lat = 43.65, Value = 6.4 },
    ];

    /// <summary>Sites of three kinds, for categorical marker colour and clustering.</summary>
    public static readonly MarkerDatum[] Sites =
    [
        Site("Rotterdam", 4.48, 51.92, "port"),
        Site("Antwerp", 4.40, 51.22, "port"),
        Site("Hamburg", 9.99, 53.55, "port"),
        Site("Bremerhaven", 8.58, 53.54, "port"),
        Site("Le Havre", 0.11, 49.49, "port"),
        Site("Felixstowe", 1.35, 51.96, "port"),
        Site("Duisburg", 6.76, 51.43, "depot"),
        Site("Cologne", 6.96, 50.94, "depot"),
        Site("Düsseldorf", 6.77, 51.23, "depot"),
        Site("Essen", 7.01, 51.46, "depot"),
        Site("Dortmund", 7.47, 51.51, "depot"),
        Site("Utrecht", 5.12, 52.09, "depot"),
        Site("Eindhoven", 5.48, 51.44, "depot"),
        Site("Ghent", 3.72, 51.05, "depot"),
        Site("Lille", 3.06, 50.63, "depot"),
        Site("Milan", 9.19, 45.46, "plant"),
        Site("Turin", 7.69, 45.07, "plant"),
        Site("Barcelona", 2.17, 41.39, "plant"),
        Site("Madrid", -3.70, 40.42, "plant"),
        Site("Prague", 14.44, 50.08, "plant"),
        Site("Warsaw", 21.01, 52.23, "plant"),
        Site("Budapest", 19.04, 47.50, "plant"),
    ];

    private static MarkerDatum Site(string name, double lon, double lat, string kind)
        => new() { Name = name, Lon = lon, Lat = lat, Extra = new() { ["kind"] = kind } };

    /// <summary>Long-haul routes out of two hubs, weighted by weekly seats.</summary>
    public static readonly ArcDatum[] Flights =
    [
        Flight("Singapore", "London", 103.99, 1.36, -0.46, 51.47, 21_400),
        Flight("Singapore", "Frankfurt", 103.99, 1.36, 8.57, 50.03, 14_900),
        Flight("Singapore", "Sydney", 103.99, 1.36, 151.18, -33.95, 28_600),
        Flight("Singapore", "Tokyo", 103.99, 1.36, 140.39, 35.77, 19_800),
        Flight("Singapore", "San Francisco", 103.99, 1.36, -122.38, 37.62, 8_200),
        Flight("Singapore", "Johannesburg", 103.99, 1.36, 28.24, -26.13, 5_100),
        Flight("Dubai", "London", 55.36, 25.25, -0.46, 51.47, 32_700),
        Flight("Dubai", "New York", 55.36, 25.25, -73.78, 40.64, 15_600),
        Flight("Dubai", "São Paulo", 55.36, 25.25, -46.47, -23.43, 6_300),
        Flight("Dubai", "Sydney", 55.36, 25.25, 151.18, -33.95, 11_200),
        Flight("Dubai", "Mumbai", 55.36, 25.25, 72.87, 19.09, 24_800),
        Flight("Dubai", "Nairobi", 55.36, 25.25, 36.93, -1.32, 7_400),
    ];

    private static ArcDatum Flight(string from, string to, double fromLon, double fromLat, double toLon, double toLat, double seats)
        => new()
        {
            Name = $"{from} - {to}",
            From = (fromLon, fromLat),
            To = (toLon, toLat),
            Value = seats,
            Extra = new() { ["hub"] = from },
        };

    /// <summary>A coastal shipping lane, as a route through its own vertices.</summary>
    public static readonly LineDatum[] ShippingLanes =
    [
        new()
        {
            Id = "suez",
            Name = "Rotterdam - Singapore, via Suez",
            Value = 3,
            Path =
            [
                new LonLat(4.10, 51.95), new LonLat(-1.50, 50.30), new LonLat(-9.50, 38.70),
                new LonLat(-5.60, 36.00), new LonLat(3.00, 37.20), new LonLat(14.50, 35.90),
                new LonLat(25.00, 33.50), new LonLat(32.35, 31.25), new LonLat(32.55, 29.95),
                new LonLat(35.50, 27.50), new LonLat(43.40, 12.60), new LonLat(51.00, 12.00),
                new LonLat(60.00, 15.00), new LonLat(72.00, 12.00), new LonLat(80.00, 5.90),
                new LonLat(95.00, 5.50), new LonLat(100.30, 2.00), new LonLat(103.85, 1.29),
            ],
        },
        new()
        {
            Id = "cape",
            Name = "Rotterdam - Singapore, via the Cape",
            Value = 1,
            Path =
            [
                new LonLat(4.10, 51.95), new LonLat(-6.00, 48.00), new LonLat(-10.00, 36.00),
                new LonLat(-17.00, 14.00), new LonLat(-9.00, -2.00), new LonLat(9.00, -25.00),
                new LonLat(18.40, -34.40), new LonLat(30.00, -35.00), new LonLat(45.00, -30.00),
                new LonLat(60.00, -20.00), new LonLat(80.00, -8.00), new LonLat(95.00, 2.00),
                new LonLat(103.85, 1.29),
            ],
        },
    ];
}
