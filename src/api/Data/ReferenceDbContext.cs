using AlasdairCooper.Reference.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using File = AlasdairCooper.Reference.Api.Data.Entities.File;

namespace AlasdairCooper.Reference.Api.Data;

public class ReferenceDbContext(DbContextOptions<ReferenceDbContext> options) : DbContext(options)
{
    // public required DbSet<File> Files { get; set; }
    //
    // public required DbSet<Media> Media { get; set; }
    
    public required DbSet<Sku> Skus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SkuEntityTypeConfiguration());
        // modelBuilder.ApplyConfiguration(new BinaryEntityTypeConfiguration());
        // modelBuilder.ApplyConfiguration(new FileEntityTypeConfiguration());
        // modelBuilder.ApplyConfiguration(new MediaEntityTypeConfiguration());
    }
}