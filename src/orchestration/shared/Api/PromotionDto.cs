namespace AlasdairCooper.Reference.Orchestration.Shared.Api;

public sealed record PromotionDto(
    int Id,
    string Name,
    string DisplayName,
    string Description,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    string[] MediaUrls);