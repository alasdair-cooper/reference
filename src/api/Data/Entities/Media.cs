using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlasdairCooper.Reference.Api.Data.Entities;

public sealed class Media(int id, string mediaType, byte[] contents, string? fileName = null) : Binary(id, contents)
{
    [StringLength(100)]
    public string MediaType { get; private set; } = mediaType;

    [StringLength(400)]
    public string? FileName { get; private set; } = fileName;
}

public class MediaEntityTypeConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.Property(x => x.FileName).HasColumnName(nameof(Media.FileName));
    }
}