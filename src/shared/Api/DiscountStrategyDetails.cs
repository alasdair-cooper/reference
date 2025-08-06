using System.Text.Json.Serialization;

namespace AlasdairCooper.Reference.Shared.Api;

[JsonDerivedType(typeof(BuyNForXDiscountStrategyDetails), "buy-n-for-x")]
public abstract record DiscountStrategyDetails;