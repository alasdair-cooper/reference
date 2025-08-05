using System.Net.Http.Json;

namespace AlasdairCooper.Reference.Orchestration.Shared.Api;

public class ApiClient(HttpClient httpClient)
{
    public async Task<List<PromotionDto>> ListPromotionsAsync(CancellationToken cancellationToken = default) => 
        await httpClient.GetFromJsonAsync<List<PromotionDto>>("promotions", cancellationToken) ?? throw new InvalidOperationException();
}