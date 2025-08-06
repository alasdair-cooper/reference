using AlasdairCooper.Reference.Components.Dialog;
using AlasdairCooper.Reference.BlazorPlayground.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents(x => x.DetailedErrors = true);

builder.Services.AddDialogs();

var app = builder.Build();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();