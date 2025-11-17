using System.Text.Json;
using Microsoft.JSInterop;

namespace AlasdairCooper.Reference.Components.Utilities.BrowserStorage;

public class SessionStorageService(IJSRuntime jsRuntime)
{
    public async Task SetItemAsync<T>(string key, T item, JsonSerializerOptions? options = null)
    {
        var json = JsonSerializer.Serialize(item, options ?? JsonSerializerOptions.Web);
        await jsRuntime.InvokeAsync<object>("sessionStorage.setItem", key, json);
    }

    public async Task<T?> GetItemAsync<T>(string key, JsonSerializerOptions? options = null)
    {
        var json = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", key);
        return json is not null ? JsonSerializer.Deserialize<T>(json, options ?? JsonSerializerOptions.Web) : default;
    }

    public async Task RemoveItemAsync(string key) => await jsRuntime.InvokeAsync<object>("sessionStorage.removeItem", key);
}