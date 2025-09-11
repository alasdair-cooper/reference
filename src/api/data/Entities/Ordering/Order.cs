using AlasdairCooper.Reference.Api.Data.Entities.Addresses;

namespace AlasdairCooper.Reference.Api.Data.Entities.Ordering;

public sealed class Order(int id)
{
    public int Id { get; private set; } = id;

    public List<Dispatch> Dispatches { get; init; } = null!;
    
    public Address Address { get; init; } = null!;
}