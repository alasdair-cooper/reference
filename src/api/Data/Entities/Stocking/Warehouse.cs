using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class Warehouse(int id, string name) : ILocatable
{
    public int Id { get; init; } = id;

    [StringLength(10)]
    public string Name { get; private set; } = name;

    public List<Aisle> Aisles { get; init; } = null!;
    
    public DistributionCenter DistributionCenter { get; init; } = null!;

    public ILocatable Parent => DistributionCenter;

    public string Key => Name;
}