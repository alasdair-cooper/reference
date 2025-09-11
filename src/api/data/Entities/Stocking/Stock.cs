using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class Stock(int id, string name) : ILocatable
{
    public int Id { get; init; } = id;
    
    [StringLength(10)]
    public string Name { get; private set; } = name;
    
    public Bin Bin { get; init; } = null!;

    public ILocatable Parent => Bin;

    public string Key => Name;
}