using AlasdairCooper.Reference.Components.Dialog;
using AlasdairCooper.Reference.BlazorPlayground.Components;
using AlasdairCooper.Reference.Components.Inputs;
using AlasdairCooper.Reference.Components.State;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents(x => x.DetailedErrors = true);

builder.Services.AddDialogs();
builder.Services.AddInputs();
builder.Services.AddState();

var app = builder.Build();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();