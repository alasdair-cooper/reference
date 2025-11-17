using AlasdairCooper.Reference.Components.Dialog;
using AlasdairCooper.Reference.BlazorPlayground.Components;
using AlasdairCooper.Reference.BlazorPlayground.Utilities;
using AlasdairCooper.Reference.Components.Inputs;
using AlasdairCooper.Reference.Components.State;
using AlasdairCooper.Reference.Components.Utilities.BrowserStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents(x => x.DetailedErrors = true);

builder.Services.AddDialogs();
builder.Services.AddInputs();
builder.Services.AddState();
builder.Services.AddBrowserStorageServices();

builder.Services.AddAuthentication();
builder.Services.AddAuthorizationBuilder().AddPolicy("test", x => x.RequireRole("test"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<PageService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();