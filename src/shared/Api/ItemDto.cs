using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Shared.Api;

public record ItemDto(
    int SkuId,
    string DisplayName,
    string? Description,
    string[] KeyPoints,
    Money ActualPrice,
    Money? PricePreDiscount,
    int? LimitedStockCount,
    string[] Discounts,
    string[] MediaUrls,
    string[] Tags);