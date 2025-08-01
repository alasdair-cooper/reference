using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using AlasdairCooper.Reference.Api.Data.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlasdairCooper.Reference.Api.Data.Entities;

public sealed class Sku(int id, string name, string description, Money price)
{
    public int Id { get; private set; } = id;

    [StringLength(100)]
    public string Name { get; private set; } = name;

    [StringLength(300)]
    public string Description { get; private set; } = description;

    public Money Price { get; private set; } = price;

    // public List<Media> Media { get; private set; } = null!;
}

public class SkuEntityTypeConfiguration : IEntityTypeConfiguration<Sku>
{
    public void Configure(EntityTypeBuilder<Sku> builder)
    {
    }
}