using Blazor_ApexMaps.Configuration;
using Blazor_ApexMaps.Interop;
using Blazor_ApexMaps.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor_ApexMaps.Extensions;

/// <summary>Registers the ApexMaps services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds ApexMaps to the container. Call it once at startup; the licence key and the geometry
    /// source are applied before the first map renders.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Services.AddApexMaps(options =>
    /// {
    ///     options.LicenseKey = "APEX-xxxxxxxx";
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddApexMaps(
        this IServiceCollection services,
        Action<ApexMapsConfiguration>? configure = null)
    {
        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.AddScoped<ApexMapsInterop>();
        services.AddScoped<ApexMapsRuntimeService>();

        return services;
    }
}
