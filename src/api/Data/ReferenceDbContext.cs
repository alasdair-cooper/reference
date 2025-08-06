using AlasdairCooper.Reference.Api.Data.Entities;
using AlasdairCooper.Reference.Api.Data.Entities.Addresses;
using AlasdairCooper.Reference.Api.Data.Entities.Discounts;
using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;
using AlasdairCooper.Reference.Api.Data.Entities.Media;
using AlasdairCooper.Reference.Api.Data.Entities.Users;
using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Orchestration.Shared.Common;
using Microsoft.EntityFrameworkCore;
using File = AlasdairCooper.Reference.Api.Data.Entities.Media.File;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(DatabaseConstants.PostgresExtensions.FuzzyStringMatch);
        modelBuilder.HasPostgresExtension(DatabaseConstants.PostgresExtensions.Trigrams);
        
        modelBuilder.Entity<User>(x => { x.HasDiscriminator().HasValue<AnonymousUser>("anon").HasValue<AuthenticatedUser>("known"); });

        modelBuilder.Entity<AuthenticatedUser>(
            x =>
            {
                x.HasOne(x => x.DeliveryAddress).WithOne().HasForeignKey<AuthenticatedUser>();
                x.HasMany(x => x.Addresses).WithMany();
            });

        modelBuilder.Entity<Country>(x => { x.HasData(new Country(1, "United Kingdom", "GB"), new Country(2, "United States of America", "US")); });

        modelBuilder.Entity<Address>(
            x =>
            {
                x.HasDiscriminator(x => x.CountryId).HasValue<UkAddress>(1);
                x.Navigation(x => x.Country).AutoInclude();
            });

        modelBuilder.Entity<Discount>(x => x.HasDiscriminator().HasValue<SkuDiscount>("sku").HasValue<TagDiscount>("tag"));

        modelBuilder.Entity<DiscountStrategy>(
            x =>
                x.HasDiscriminator()
                    .HasValue<FlatValueDiscountStrategy>("flat-value")
                    .HasValue<FlatPercentageDiscountStrategy>("flat-percent")
                    .HasValue<BuyNForXPriceDiscountStrategy>("buy-n-for-x")
                    .HasValue<BuyNGetMFreeDiscountStrategy>("buy-n-get-m-free")
                    .HasValue<SpendXGetYPercentOffDiscountStrategy>("spend-x-get-y-off"));

        modelBuilder.Entity<File>(x => x.HasDiscriminator().HasValue<Media>("media"));
    }

    public async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        Tags.RemoveRange(Tags);

        var paintsTag = new Tag(0, "category:paints");
        var miniaturesTag = new Tag(0, "category:miniatures");
        var spaceMarinesTag = new Tag(0, "faction:space-marines");

        Add(paintsTag);
        Add(miniaturesTag);
        Add(spaceMarinesTag);

        Skus.RemoveRange(Skus);

        var intercessors = new Sku(0, "Intercessors", [], null, Money.FromValue(CurrencyType.Gbp, 40)) { Tags = [miniaturesTag, spaceMarinesTag] };

        var terminatorSquad =
            new Sku(0, "Terminator Squad", [], null, Money.FromValue(CurrencyType.Gbp, 42.50)) { Tags = [miniaturesTag, spaceMarinesTag] };

        var baalRed = new Sku(0, "Baal Red", [], null, Money.FromValue(CurrencyType.Gbp, 4.75)) { Tags = [paintsTag] };

        var leviathanPurple = new Sku(0, "Leviathan Purple", [], null, Money.FromValue(CurrencyType.Gbp, 4.75)) { Tags = [paintsTag] };

        var druchiiViolet = new Sku(0, "Druchii Violet", [], null, Money.FromValue(CurrencyType.Gbp, 4.75)) { Tags = [paintsTag] };

        Add(intercessors);
        Add(terminatorSquad);
        Add(baalRed);
        Add(leviathanPurple);
        Add(druchiiViolet);

        DiscountStrategies.RemoveRange(DiscountStrategies);

        var flatPercentageStrategy = new FlatPercentageDiscountStrategy(0, BoundedPercentage.FromPercentage(5));
        var buy1Get1FreeStrategy = new BuyNGetMFreeDiscountStrategy(0, 1, 1);

        Add(flatPercentageStrategy);
        Add(buy1Get1FreeStrategy);

        Discounts.RemoveRange(Discounts);

        var spaceMarinesDiscount = new TagDiscount(0) { Strategy = flatPercentageStrategy, Tags = [spaceMarinesTag] };

        var paintsDiscount = new SkuDiscount(0) { Strategy = buy1Get1FreeStrategy, Skus = [baalRed, leviathanPurple, druchiiViolet] };

        Add(spaceMarinesDiscount);
        Add(paintsDiscount);

        Promotions.RemoveRange(Promotions);

        var summerSale =
            new Promotion(
                0,
                "summer-sale",
                "Summer Sale",
                "Get 5% off all Space Marines and buy 1 get 1 free on a selection of paints",
                new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero)) { Discounts = [spaceMarinesDiscount, paintsDiscount] };

        Add(summerSale);
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }
}