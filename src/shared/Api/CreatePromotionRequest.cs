namespace AlasdairCooper.Reference.Shared.Api;

public sealed record CreatePromotionRequest(
    string Name,
    string DisplayName,
    string Description,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    DiscountDetails[] Discounts,
    int[] MediaIds);