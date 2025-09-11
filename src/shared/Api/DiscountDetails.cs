namespace AlasdairCooper.Reference.Shared.Api;

public sealed record DiscountDetails(DiscountStrategyDetails Strategy, int[] SkuIds, int[] TagIds);