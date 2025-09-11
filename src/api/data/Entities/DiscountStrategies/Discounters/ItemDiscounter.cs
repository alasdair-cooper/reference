using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies.Discounters;

public sealed record ItemDiscounter(PricedItem Item, Func<Money, Money> ApplyDiscount) : Discounter;