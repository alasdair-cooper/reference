using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;

namespace AlasdairCooper.Reference.Api.Data.Entities.Discounts;

public abstract class Discount(int id)
{
    public int Id { get; init; } = id;

    public DiscountStrategy Strategy { get; init; } = null!;
}