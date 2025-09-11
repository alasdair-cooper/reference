namespace AlasdairCooper.Reference.Api.Data.Entities.Discounts;

public sealed class SkuDiscount(int id) : Discount(id)
{
    public List<Sku> Skus { get; init; } = null!;
}