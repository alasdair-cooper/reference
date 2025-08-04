namespace AlasdairCooper.Reference.Orchestration.Shared.Api;

public sealed record DiscountDetails(DiscountStrategyDetails Strategy, int[] SkuIds, int[] TagIds);