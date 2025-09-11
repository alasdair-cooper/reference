using AlasdairCooper.Reference.Api.Data.Entities;
using AlasdairCooper.Reference.Api.Data.Entities.Addresses;
using AlasdairCooper.Reference.Api.Data.Entities.Content;
using AlasdairCooper.Reference.Api.Data.Entities.Discounts;
using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;
using AlasdairCooper.Reference.Api.Data.Entities.Ordering;
using AlasdairCooper.Reference.Api.Data.Entities.Stocking;
using AlasdairCooper.Reference.Api.Data.Entities.Users;
using AlasdairCooper.Reference.Api.Data.Utilities;
using Microsoft.EntityFrameworkCore;
using File = AlasdairCooper.Reference.Api.Data.Entities.Content.File;

namespace AlasdairCooper.Reference.Api.Data;

public class ReferenceDbContext(DbContextOptions<ReferenceDbContext> options) : DbContext(options)
{
    #region Users

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Address> Addresses { get; set; } = null!;

    public DbSet<Basket> Baskets { get; set; } = null!;

    public DbSet<BasketItem> BasketItems { get; set; } = null!;

    public DbSet<Country> Countries { get; set; } = null!;

    #endregion

    #region Products

    public DbSet<Sku> Skus { get; set; } = null!;

    public DbSet<Tag> Tags { get; set; } = null!;

    public DbSet<Promotion> Promotions { get; set; } = null!;

    public DbSet<Discount> Discounts { get; set; } = null!;

    public DbSet<DiscountStrategy> DiscountStrategies { get; set; } = null!;

    #endregion

    #region Media

    public DbSet<File> Files { get; set; } = null!;

    #endregion

    #region Stock

    public DbSet<Stock> Stock { get; set; } = null!;
    
    public DbSet<Bin> Bins { get; set; } = null!;
    
    public DbSet<Rack> Racks { get; set; } = null!;
    
    public DbSet<Aisle> Aisles { get; set; } = null!;
    
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    
    public DbSet<DistributionCenter> DistributionCenters { get; set; } = null!;
    
    #endregion
    
    #region Ordering
    
    public DbSet<Order> Orders { get; set; } = null!;
    
    public DbSet<Dispatch> Dispatches { get; set; } = null!;
    
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(DatabaseConstants.PostgresExtensions.FuzzyStringMatch);
        modelBuilder.HasPostgresExtension(DatabaseConstants.PostgresExtensions.Trigrams);
        
        modelBuilder.Entity<User>(
            static x =>
            {
                x.HasDiscriminator().HasValue<AnonymousUser>("anon").HasValue<AuthenticatedUser>("known");
                x.HasOne(static x => x.Basket).WithOne(static x => x.User).HasForeignKey<Basket>();
            });

        modelBuilder.Entity<AuthenticatedUser>(
            static x =>
            {
                x.HasOne(static x => x.DeliveryAddress).WithOne().HasForeignKey<AuthenticatedUser>();
                x.HasMany(static x => x.Addresses).WithMany();
            });

        modelBuilder.Entity<Country>(static x => { x.HasData(new Country(1, "United Kingdom", "GB"), new Country(2, "United States of America", "US")); });

        modelBuilder.Entity<Address>(
            static x =>
            {
                x.HasDiscriminator(static x => x.CountryId).HasValue<UkAddress>(1);
                x.Navigation(static x => x.Country).AutoInclude();
            });

        modelBuilder.Entity<Discount>(static x => x.HasDiscriminator().HasValue<SkuDiscount>("sku").HasValue<TagDiscount>("tag"));

        modelBuilder.Entity<DiscountStrategy>(
            static x =>
                x.HasDiscriminator()
                    .HasValue<FlatValueDiscountStrategy>("flat-value")
                    .HasValue<FlatPercentageDiscountStrategy>("flat-percent")
                    .HasValue<BuyNForXPriceDiscountStrategy>("buy-n-for-x")
                    .HasValue<BuyNGetMFreeDiscountStrategy>("buy-n-get-m-free")
                    .HasValue<SpendXGetYPercentOffDiscountStrategy>("spend-x-get-y-off"));

        modelBuilder.Entity<File>(static x => x.HasDiscriminator().HasValue<Media>("media"));
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }
}