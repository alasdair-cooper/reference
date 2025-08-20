using System.ComponentModel.DataAnnotations;
using AlasdairCooper.Reference.Api.Data.Entities.Content;
using AlasdairCooper.Reference.Api.Data.Entities.Discounts;

namespace AlasdairCooper.Reference.Api.Data.Entities;

public sealed class Promotion(int id, string name, string displayName, string description, DateTimeOffset? startDate, DateTimeOffset? endDate)
{
    public int Id { get; init; } = id;
    
    [StringLength(100)]
    public string Name { get; init; } = name;
    
    [StringLength(100)]
    public string DisplayName { get; init; } = displayName;
    
    [StringLength(400)]
    public string Description { get; init; } = description;

    public DateTimeOffset? StartDate { get; init; } = startDate;

    public DateTimeOffset? EndDate { get; init; } = endDate;

    public List<Discount> Discounts { get; init; } = null!;

    public List<Media> Media { get; init; } = null!;
}