using System.ComponentModel.DataAnnotations;
using AlasdairCooper.Reference.Api.Data.Entities.Addresses;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class DistributionCenter(int id, string name) : ILocatable
{
    public int Id { get; init; } = id;

    [StringLength(10)]
    public string Name { get; private set; } = name;

    public List<Warehouse> Warehouses { get; init; } = null!;

    public Address Address { get; init; } = null!;

    public ILocatable? Parent => null;

    public string Key => Name;
}