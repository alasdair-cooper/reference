using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlasdairCooper.Reference.Api.Data.Entities;

public abstract class Binary(int id, byte[] contents)
{
    public int Id { get; private set; } = id;

    public byte[] Contents { get; private set; } = contents;
}

public class BinaryEntityTypeConfiguration : IEntityTypeConfiguration<Binary>
{
    public void Configure(EntityTypeBuilder<Binary> builder)
    {
        builder.HasDiscriminator().HasValue<Media>("media").HasValue<File>("file");
    }
}