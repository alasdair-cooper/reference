using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class Rack(int id, string name) : ILocatable
{
    public int Id { get; init; } = id;

    [StringLength(10)]
    public string Name { get; private set; } = name;

    public List<Bin> Bins { get; init; } = null!;

    public Aisle Aisle { get; init; } = null!;

    public ILocatable Parent => Aisle;

    public string Key => Name;
}