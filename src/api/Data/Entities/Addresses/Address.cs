namespace AlasdairCooper.Reference.Api.Data.Entities.Addresses;

public abstract class Address(int id)
{
    public int Id { get; init; } = id;

    public int CountryId { get; init; }

    public Country Country { get; init; } = null!;

    public abstract string ToDisplayString();
}