using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies.Discounters;
using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;

public class BuyNGetMFreeDiscountStrategy(int id, int n, int m) : DiscountStrategy(id)
{
    public int N { get; init; } = n;

    public int M { get; init; } = m;

    public override string ToDisplayString() => $"Buy {N} get {M} free";

    public override IEnumerable<Discounter> CalculateDiscount(IEnumerable<PricedItem> items, DiscountContext context) =>
        items.Chunk(N + M)
            .Where(x => x.Length == N + M)
            .SelectMany(x => x.Skip(N).Take(M).Select(x => new ItemDiscounter(x, x => Money.Zero(x.Currency))));
}