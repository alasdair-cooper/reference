using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies.Discounters;
using AlasdairCooper.Reference.Api.Data.Utilities;

namespace AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;

public abstract class DiscountStrategy(int id)
{
    public int Id { get; init; } = id;
    
    public abstract string ToDisplayString();
    
    public abstract IEnumerable<Discounter> CalculateDiscount(IEnumerable<PricedItem> items, DiscountContext context);
}