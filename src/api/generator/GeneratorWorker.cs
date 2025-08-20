using System.Diagnostics;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Data.Entities;
using AlasdairCooper.Reference.Api.Data.Entities.Addresses;
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

        await strategy.ExecuteAsync(
            (dbContext, cancellationToken),
            static async state =>
            {
                await using var transaction = await state.dbContext.Database.BeginTransactionAsync(state.cancellationToken);

                SeedUsers(state.dbContext);
                var bins = SeedLocations(state.dbContext);
                SeedProducts(state.dbContext, bins);

                await state.dbContext.SaveChangesAsync(state.cancellationToken);

                await transaction.CommitAsync(state.cancellationToken);
            });
    }

    private static void SeedUsers(ReferenceDbContext dbContext)
    {
        var faker =
            new Faker<AuthenticatedUser>().UseSeed(123)
                .CustomInstantiator(
                    static f =>
                        new AuthenticatedUser(
                            0,
                            f.Name.FirstName(),
                            f.Make(f.Random.Int(0, 2), _ => f.Name.LastName()).ToArray(),
                            f.Name.LastName()));

        foreach (var user in faker.GenerateLazy(500))
        {
            dbContext.Add(user);
        }
    }

    private static IEnumerable<Bin> SeedLocations(ReferenceDbContext dbContext)
    {
        var addressFaker =
            new Faker<UkAddress>().UseSeed(123)
                .CustomInstantiator(
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

    private static void SeedProducts(ReferenceDbContext dbContext, IEnumerable<Bin> bins)
    {
        var skuFaker =
            new Faker<Sku>().UseSeed(123)
                .CustomInstantiator(
                    static f =>
                        new Sku(
                            0,
                            f.Commerce.ProductName(),
                            [f.Commerce.ProductAdjective(), f.Commerce.ProductAdjective(), f.Commerce.ProductAdjective()],
                            f.Commerce.ProductDescription(),
                            Money.FromValue(CurrencyType.Gbp, f.Random.Number(5, 20) - 0.01m)) { Bins = [] });

        var stockFaker = new Faker<Stock>().UseSeed(123).CustomInstantiator(static f => new Stock(0, f.Commerce.Ean8()));

        foreach (var (sku, bin) in skuFaker.GenerateLazy(2000)
            .SelectMany(static x => Enumerable.Range(0, new Faker().Random.Number(1, 8)).Select(_ => x))
            .Zip(bins))
        {
            var stock = stockFaker.GenerateBetween(1, 300);
            bin.AddStock(sku, stock);
            dbContext.Add(sku);
        }
    }
}