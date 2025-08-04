namespace AlasdairCooper.Reference.Orchestration.Shared.Api;

public sealed record DiscountDto(int Id, string Description, int[] SkuIds, int[] TagIds);