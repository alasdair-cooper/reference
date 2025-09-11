using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class Aisle(int id, string name) : ILocatable
{
    public int Id { get; init; } = id;

    [StringLength(10)]
    public string Name { get; private set; } = name;

    public List<Rack> Racks { get; init; } = null!;

    public Warehouse Warehouse { get; init; } = null!;

    public ILocatable Parent => Warehouse;

    public string Key => Name;
}