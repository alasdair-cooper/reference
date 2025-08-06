using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies.Discounters;
using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;

public sealed class FlatValueDiscountStrategy(int id, Money amount) : DiscountStrategy(id)
{
    public Money Amount { get; init; } = amount;

    public override string ToDisplayString() => $"{Amount.ToDisplayString()} off";

    public override IEnumerable<Discounter> CalculateDiscount(IEnumerable<PricedItem> items, DiscountContext context) =>
        [new GlobalDiscounter(x => x - Amount)];
}