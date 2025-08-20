namespace AlasdairCooper.Reference.Api.Data.Entities;

public sealed class Basket(int id)
{
    public int Id { get; init; } = id;

    public List<BasketItem> Items { get; init; } = null!;
}