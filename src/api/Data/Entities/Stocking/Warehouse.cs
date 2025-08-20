using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class Warehouse(int id) : ILocatable
{
    public int Id { get; private set; } = id;

    [StringLength(10)]
    public string Name { get; private set; } = null!;

    public List<Aisle> Aisles { get; init; } = null!;
    
    public DistributionCenter DistributionCenter { get; init; } = null!;

    public ILocatable Parent => DistributionCenter;

    public string Key => Name;
}