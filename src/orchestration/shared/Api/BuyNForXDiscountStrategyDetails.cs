using AlasdairCooper.Reference.Orchestration.Shared.Common;

namespace AlasdairCooper.Reference.Orchestration.Shared.Api;

public sealed record BuyNForXDiscountStrategyDetails(int N, Money X) : DiscountStrategyDetails;