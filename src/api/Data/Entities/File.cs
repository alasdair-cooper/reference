using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlasdairCooper.Reference.Api.Data.Entities;

public sealed class File(int id, string fileName, byte[] contents) : Binary(id, contents)
{
    [StringLength(400)]
    public string FileName { get; private set; } = fileName;
}

public class FileEntityTypeConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.Property(x => x.FileName).HasColumnName(nameof(File.FileName));
    }
}