using System.Text.Json.Serialization;

namespace AlasdairCooper.Reference.Orchestration.Shared.Api;

[JsonDerivedType(typeof(BuyNForXDiscountStrategyDetails), "buy-n-for-x")]
public abstract record DiscountStrategyDetails;