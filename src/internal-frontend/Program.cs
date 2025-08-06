using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AlasdairCooper.Reference.InternalFrontend;
using AlasdairCooper.Reference.Shared.Api;
using AlasdairCooper.Reference.Shared.Orchestration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.AddServiceDefaults();

builder.Services.ConfigureHttpClientDefaults(x => x.AddServiceDiscovery());

builder.Services.AddHttpClient<ApiClient>(x => x.BaseAddress = new Uri($"https+http://{AspireConstants.Resources.Api}"));

await builder.Build().RunAsync();