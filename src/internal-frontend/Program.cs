using System.Security.Claims;
using AlasdairCooper.Reference.Components.Dialog;
using AlasdairCooper.Reference.Components.Inputs;
using AlasdairCooper.Reference.Components.State;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AlasdairCooper.Reference.InternalFrontend;
using AlasdairCooper.Reference.Shared.Api;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddDialogsAsSingleton();
builder.Services.AddInputsAsSingleton();
builder.Services.AddState();

builder.Services.AddSingleton<AuthenticationStateProvider, NoAuthAuthenticationStateProvider>();

builder.AddServiceDefaults();

// builder.Services.AddHttpClient<ApiClient>(static x => x.BaseAddress = new Uri($"https+http://{AspireConstants.Resources.Api}"));
builder.Services.AddHttpClient<ApiClient>(static x => x.BaseAddress = new Uri("https://localhost:7101"));

var app = builder.Build();

await app.RunAsync();

internal sealed class NoAuthAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity([], "no_auth"))));
}