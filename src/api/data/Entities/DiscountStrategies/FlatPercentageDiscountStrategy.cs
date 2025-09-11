using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies.Discounters;
using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;

public sealed class FlatPercentageDiscountStrategy(int id, BoundedPercentage percentage) : DiscountStrategy(id)
{
    public BoundedPercentage Percentage { get; init; } = percentage;

    public override string ToDisplayString() => $"{Percentage.ToDisplayString()} off";

    public override IEnumerable<Discounter> CalculateDiscount(IEnumerable<PricedItem> items, DiscountContext context) => 
        items.Select(item => new ItemDiscounter(item, x => x - x * Percentage));
}