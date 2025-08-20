using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class Aisle(int id) : ILocatable
{
    public int Id { get; private set; } = id;

    [StringLength(10)]
    public string Name { get; private set; } = null!;

    public List<Rack> Racks { get; init; } = null!;

    public Warehouse Warehouse { get; init; } = null!;

    public ILocatable Parent => Warehouse;

    public string Key => Name;
}