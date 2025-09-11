namespace AlasdairCooper.Reference.Api.Data.Entities;

public sealed class BasketItem(int id, int quantity)
{
    public int Id { get; init; } = id;

    public required Sku Sku { get; init; } = null!;

    public int Quantity { get; private set; } = quantity;

    public void DecrementQuantity() => Quantity--;

    public void IncrementQuantity() => Quantity++;
}