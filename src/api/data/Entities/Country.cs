using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities;

public sealed class Country(int id, string name, string code)
{
    public int Id { get; init; } = id;

    [StringLength(100)]
    public string Name { get; init; } = name;

    [StringLength(2)]
    public string Code { get; init; } = code;

    public string ToDisplayString() => Name;
}