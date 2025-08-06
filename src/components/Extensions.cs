using Microsoft.JSInterop;

namespace AlasdairCooper.Reference.Components;

public static class Extensions
{
    public static async Task<IJSObjectReference> GetModule(this IJSRuntime jsRuntime, string path) => 
        await jsRuntime.InvokeAsync<IJSObjectReference>("import", path);
}