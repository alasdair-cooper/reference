using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class Rack(int id) : ILocatable
{
    public int Id { get; private set; } = id;

    [StringLength(10)]
    public string Name { get; private set; } = null!;

    public List<Bin> Bins { get; init; } = null!;

    public Aisle Aisle { get; init; } = null!;

    public ILocatable Parent => Aisle;

    public string Key => Name;
}