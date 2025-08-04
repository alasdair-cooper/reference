using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities;

public sealed class Tag(int id, string name)
{
    public int Id { get; init; } = id;

    [StringLength(100)]
    public string Name { get; init; } = name;

    public List<Sku> Skus { get; init; } = null!;
}