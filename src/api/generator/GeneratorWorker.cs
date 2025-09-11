using System.Diagnostics;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Data.Entities;
using AlasdairCooper.Reference.Api.Data.Entities.Addresses;
using AlasdairCooper.Reference.Api.Data.Entities.Discounts;
using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;
using AlasdairCooper.Reference.Api.Data.Entities.Stocking;
using AlasdairCooper.Reference.Api.Data.Entities.Users;
using AlasdairCooper.Reference.Shared.Common;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace AlasdairCooper.Reference.Api.Generator;

public class GeneratorWorker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Generator";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ReferenceDbContext>();

            await dbContext.Database.EnsureDeletedAsync(cancellationToken);
            await RunMigrationAsync(dbContext, cancellationToken);

            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(ReferenceDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            (dbContext, cancellationToken),
            // Run migration in a transaction to avoid partial migration if it fails.
            static async state => await state.dbContext.Database.MigrateAsync(state.cancellationToken));
    }

    private static async Task SeedDataAsync(ReferenceDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        Randomizer.Seed = new Random(123);

        await strategy.ExecuteAsync(
            (dbContext, cancellationToken),
            static async state =>
            {
                await using var transaction = await state.dbContext.Database.BeginTransactionAsync(state.cancellationToken);

                SeedUsers(state.dbContext);
                var tags = SeedTags(state.dbContext);
                var bins = SeedLocations(state.dbContext);
                SeedProducts(state.dbContext, bins, tags);
                SeedPromotions(state.dbContext, tags);

                await state.dbContext.SaveChangesAsync(state.cancellationToken);

                await transaction.CommitAsync(state.cancellationToken);
            });
    }

    private static void SeedUsers(ReferenceDbContext dbContext)
    {
        var userFaker =
            new Faker<AuthenticatedUser>().CustomInstantiator(
                static f =>
                    new AuthenticatedUser(0, f.Name.FirstName(), f.Make(f.Random.Int(0, 2), _ => f.Name.LastName()).ToArray(), f.Name.LastName()));

        foreach (var user in userFaker.GenerateLazy(500))
        {
            dbContext.Add(user);
        }
    }

    private static IEnumerable<Bin> SeedLocations(ReferenceDbContext dbContext)
    {
        var addressFaker =
            new Faker<UkAddress>().CustomInstantiator(
                static f =>
                    new UkAddress(
                        0,
                        $"{f.Address.BuildingNumber()} {f.Address.StreetName()}",
                        null,
                        null,
                        f.Address.City(),
                        f.Address.County(),
                        Postcode.FromString(f.Address.ZipCode("??# #??"))));

        var distributionCenter = new DistributionCenter(0, "Main") { Address = addressFaker.Generate(1).First() };

        dbContext.DistributionCenters.Add(distributionCenter);

        return GetBins();

        IEnumerable<Bin> GetBins()
        {
            var count = 0;

            while (true)
            {
                var warehouse = new Warehouse(0, $"W{count}") { Aisles = [], DistributionCenter = distributionCenter };
                dbContext.Warehouses.Add(warehouse);

                foreach (var aisleIndex in Enumerable.Range(0, 16))
                {
                    var aisle = new Aisle(0, $"A{aisleIndex}") { Racks = [] };
                    warehouse.Aisles.Add(aisle);

                    foreach (var rackIndex in Enumerable.Range(0, 16))
                    {
                        var rack = new Rack(0, $"R{rackIndex}") { Bins = [] };
                        aisle.Racks.Add(rack);

                        foreach (var binIndex in Enumerable.Range(0, 16))
                        {
                            var bin = new Bin(0, $"B{binIndex}") { Stock = [] };
                            rack.Bins.Add(bin);
                            yield return bin;
                        }
                    }
                }

                count++;
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }

    private static List<Tag> SeedTags(ReferenceDbContext dbContext)
    {
        var collectionTagFaker = new Faker<Tag>().CustomInstantiator(static f => new Tag(0, $"collection:{f.Hacker.Adjective().ToLower()}"));
        var categoryTagFaker = new Faker<Tag>().CustomInstantiator(static f => new Tag(0, $"category:{f.Hacker.Noun().ToLower()}"));

        var collectionTags = collectionTagFaker.Generate(100);
        var categoryTags = categoryTagFaker.Generate(25);

        foreach (var collectionTag in collectionTags)
        {
            dbContext.Tags.Add(collectionTag);
        }

        foreach (var categoryTag in categoryTags)
        {
            dbContext.Tags.Add(categoryTag);
        }

        return collectionTags.Concat(categoryTags).ToList();
    }

    private static void SeedProducts(ReferenceDbContext dbContext, IEnumerable<Bin> bins, IEnumerable<Tag> tags)
    {
        var skuFaker =
            new Faker<Sku>().CustomInstantiator(
                f =>
                    new Sku(
                        0,
                        f.Commerce.ProductName(),
                        [f.Commerce.ProductAdjective(), f.Commerce.ProductAdjective(), f.Commerce.ProductAdjective()],
                        f.Commerce.ProductDescription(),
                        Money.FromValue(CurrencyType.Gbp, f.Random.Number(5, 20) - 0.01m)) { Bins = [], Tags = f.PickRandom(tags, 3).ToList() });

        var stockFaker = new Faker<Stock>().CustomInstantiator(static f => new Stock(0, f.Commerce.Ean8()));

        foreach (var (sku, bin) in skuFaker.GenerateLazy(800)
            .SelectMany(static x => Enumerable.Range(0, new Faker().Random.Number(1, 4)).Select(_ => x))
            .Zip(bins))
        {
            var stock = stockFaker.GenerateBetween(1, 25);
            bin.AddStock(sku, stock);
            dbContext.Add(sku);
        }
    }

    private static void SeedPromotions(ReferenceDbContext dbContext, List<Tag> tags)
    {
        var tenPercentOffDiscountStrategy = new FlatPercentageDiscountStrategy(0, BoundedPercentage.FromPercentage(25));
        var twentyPercentOffDiscountStrategy = new FlatPercentageDiscountStrategy(0, BoundedPercentage.FromPercentage(20));
        var twentyFivePercentOffDiscountStrategy = new FlatPercentageDiscountStrategy(0, BoundedPercentage.FromPercentage(25));
        var buyOneGetOneFreeDiscountStrategy = new BuyNGetMFreeDiscountStrategy(0, 1, 1);
        var buyThreeForTenPoundsDiscountStrategy = new BuyNForXPriceDiscountStrategy(0, 1, Money.FromValue(CurrencyType.Gbp, 10));

        List<DiscountStrategy> discountStrategies =
        [
            tenPercentOffDiscountStrategy,
            twentyPercentOffDiscountStrategy,
            twentyFivePercentOffDiscountStrategy,
            buyOneGetOneFreeDiscountStrategy,
            buyThreeForTenPoundsDiscountStrategy
        ];

        dbContext.DiscountStrategies.AddRange(discountStrategies);

        var discountFaker =
            new Faker<Discount>().CustomInstantiator(
                f => new TagDiscount(0) { Strategy = f.PickRandom(discountStrategies), Tags = f.PickRandom(tags, 3).ToList() });

        foreach (var year in Enumerable.Range(DateTimeOffset.Now.Year - 5, 10))
        {
            var newYearsSale =
                new Promotion(
                    0,
                    $"new-year-{year}",
                    "New Years Sale",
                    $"{year} New Years Sale",
                    new DateTimeOffset(year, 11, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset),
                    new DateTimeOffset(year, 12, 31, 23, 59, 59, TimeZoneInfo.Local.BaseUtcOffset))
                {
                    Discounts = discountFaker.GenerateBetween(3, 5)
                };

            var summerSale =
                new Promotion(
                    0,
                    $"summer-sale-{year}",
                    "Summer Sale",
                    $"{year} Summer Sale",
                    new DateTimeOffset(year, 8, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset),
                    new DateTimeOffset(year, 8, 31, 23, 59, 59, TimeZoneInfo.Local.BaseUtcOffset))
                {
                    Discounts = discountFaker.GenerateBetween(3, 5)
                };

            var backToSchoolSale =
                new Promotion(
                    0,
                    $"back-to-school-{year}",
                    "Back To School Sale",
                    $"{year} Back To School Sale",
                    new DateTimeOffset(year, 9, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset),
                    new DateTimeOffset(year, 9, 30, 23, 59, 59, TimeZoneInfo.Local.BaseUtcOffset))
                {
                    Discounts = discountFaker.GenerateBetween(1, 3)
                };

            var christmasSale =
                new Promotion(
                    0,
                    $"xmas-{year}",
                    "Christmas Sale",
                    $"{year} Christmas Sale",
                    new DateTimeOffset(year, 11, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset),
                    new DateTimeOffset(year, 12, 31, 23, 59, 59, TimeZoneInfo.Local.BaseUtcOffset))
                {
                    Discounts = discountFaker.GenerateBetween(5, 10)
                };

            dbContext.Promotions.Add(newYearsSale);
            dbContext.Promotions.Add(summerSale);
            dbContext.Promotions.Add(backToSchoolSale);
            dbContext.Promotions.Add(christmasSale);
        }
    }
}