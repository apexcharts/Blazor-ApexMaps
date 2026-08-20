using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor_ApexMaps.Sample;
using Blazor_ApexMaps.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddApexMaps(options =>
{
    // Licence for the public demo hosted at https://apexcharts.github.io (domain-locked, so the
    // licensed features render without a watermark there). Running this sample locally without a
    // key of your own is fine: the same features work, watermarked.
    options.LicenseKey = "APEX-eyJpc3N1ZURhdGUiOiIyMDI2LTA3LTA3IiwiZXhwaXJ5RGF0ZSI6IjIwNTAtMDctMDciLCJwbGFuIjoiZW50ZXJwcmlzZSIsImRvbWFpbnMiOlsiYXBleGNoYXJ0cy5naXRodWIuaW8iXX0=";
});

await builder.Build().RunAsync();
