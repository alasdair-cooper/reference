using AlasdairCooper.Reference.Api.Data.Entities.Users;

namespace AlasdairCooper.Reference.Api.Data.Entities;

public sealed class Basket(int id)
{
    public int Id { get; init; } = id;

    public List<BasketItem> Items { get; init; } = null!;

    public User User { get; init; } = null!;
}