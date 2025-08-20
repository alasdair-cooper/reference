using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class Stock(int id) : ILocatable
{
    public int Id { get; private set; } = id;
    
    [StringLength(10)]
    public string Name { get; private set; } = null!;
    
    public Bin Bin { get; init; } = null!;

    public ILocatable Parent => Bin;

    public string Key => Name;
}