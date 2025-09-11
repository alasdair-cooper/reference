namespace AlasdairCooper.Reference.Api.Data.Entities.Discounts;

public sealed class TagDiscount(int id) : Discount(id)
{
    public List<Tag> Tags { get; init; } = null!;
}