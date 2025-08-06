using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies.Discounters;
using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;

public class SpendXGetYPercentOffDiscountStrategy(int id, Money x, BoundedPercentage y) : DiscountStrategy(id)
{
    public Money X { get; init; } = x;

    public BoundedPercentage Y { get; init; } = y;

    public override string ToDisplayString() => 
        $"Spend {X.ToDisplayString()}, get {Y.ToDisplayString()} off";

    public override IEnumerable<Discounter> CalculateDiscount(IEnumerable<PricedItem> items, DiscountContext context) => 
        items.Sum(x => x.Price) >= X.Value ? [new GlobalDiscounter(x => x - x * Y)] : [];
}