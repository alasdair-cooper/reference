using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies.Discounters;
using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;

public sealed class BuyNForXPriceDiscountStrategy(int id, int n, Money x) : DiscountStrategy(id)
{
    public int N { get; init; } = n;

    public Money X { get; init; } = x;

    public override string ToDisplayString() => $"Buy {N} for {X.ToDisplayString()}";

    public override IEnumerable<Discounter> CalculateDiscount(IEnumerable<PricedItem> items, DiscountContext context) => 
        items.Chunk(N).Where(x => x.Length == N).SelectMany(x => x.Select(x => new ItemDiscounter(x, _ => X / N)));
}