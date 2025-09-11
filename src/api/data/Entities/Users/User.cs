namespace AlasdairCooper.Reference.Api.Data.Entities.Users;

public abstract class User(int id)
{
    public int Id { get; init; } = id;
    
    public Basket Basket { get; init; } = null!;
}