using System.ComponentModel.DataAnnotations;
using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Entities;

public sealed class Sku(int id, string displayName, string[] keyPoints, string? description, Money suggestedPrice)
{
    public int Id { get; init; } = id;

    [StringLength(100)]
    public string DisplayName { get; set; } = displayName;

    public string[] KeyPoints { get; set; } = keyPoints;

    [StringLength(400)]
    public string? Description { get; set; } = description;

    public Money SuggestedPrice { get; init; } = suggestedPrice;

    public List<Tag> Tags { get; init; } = null!;
}