using AlasdairCooper.Reference.Api.Data.Utilities;

namespace AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies.Discounters;

public sealed record AdditionalItemDiscounter(Func<PricedItem> AdditionalItemGenerator);