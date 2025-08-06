using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Shared.Api;

public sealed record BuyNForXDiscountStrategyDetails(int N, Money X) : DiscountStrategyDetails;