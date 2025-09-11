using AlasdairCooper.Reference.Api.Data.Entities.Stocking;

namespace AlasdairCooper.Reference.Api.Data.Entities.Ordering;

public sealed class Dispatch(int id, DispatchState state)
{
    public int Id { get; private set; } = id;
    
    public DispatchState State { get; private set; } = state;
    
    public List<Stock> Items { get; init; } = null!;
}