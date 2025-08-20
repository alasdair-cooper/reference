using System.Net.Http.Json;

namespace AlasdairCooper.Reference.Shared.Api;

public class ApiClient(HttpClient httpClient)
{
    public async Task<List<PromotionDto>> ListPromotionsAsync(CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<PromotionDto>>("promotions", cancellationToken) ?? throw new InvalidOperationException();

    public async Task<(List<UserDto>, int)>
        ListUsersAsync(int page, int pageSize, string? nameFilter = null, CancellationToken cancellationToken = default)
    {
        var res = await httpClient.GetAsync($"users?page={page}&pageSize={pageSize}&nameFilter={nameFilter}", cancellationToken);
        res.EnsureSuccessStatusCode();
        var users = await res.Content.ReadFromJsonAsync<List<UserDto>>(cancellationToken) ?? throw new InvalidOperationException();
        var totalCount = int.Parse(res.Headers.GetValues("X-Total-Count").Single());
        return (users, totalCount);
    }
    
    public async Task<(List<ItemDto>, int)>
        ListItemsAsync(int page, int pageSize, string? nameFilter = null, CancellationToken cancellationToken = default)
    {
        var res = await httpClient.GetAsync($"items?page={page}&pageSize={pageSize}&nameFilter={nameFilter}", cancellationToken);
        res.EnsureSuccessStatusCode();
        var items = await res.Content.ReadFromJsonAsync<List<ItemDto>>(cancellationToken) ?? throw new InvalidOperationException();
        var totalCount = int.Parse(res.Headers.GetValues("X-Total-Count").Single());
        return (items, totalCount);
    }
}