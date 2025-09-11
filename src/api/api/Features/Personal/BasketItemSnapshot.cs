using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Features.Personal;

public record BasketItemSnapshot(int SkuId, string DisplayName, string? ThumbnailUrl, Money? OriginalPrice, Money ActualPrice, int Count);