using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Orchestration.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies.Discounters;

public sealed record GlobalDiscounter(Func<Money, Money> ApplyDiscount) : Discounter;