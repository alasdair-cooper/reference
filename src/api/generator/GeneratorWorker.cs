using System.Diagnostics;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Data.Entities.Users;
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
            (dbContext, seeders: new List<Func<ReferenceDbContext, CancellationToken, Task>> { SeedUsersAsync }, cancellationToken),
            static async state =>
            {
                await using var transaction = await state.dbContext.Database.BeginTransactionAsync(state.cancellationToken);

                foreach (var seeder in state.seeders)
                {
                    await seeder.Invoke(state.dbContext, state.cancellationToken);
                    await state.dbContext.SaveChangesAsync(state.cancellationToken);
                }

                await transaction.CommitAsync(state.cancellationToken);
            });
    }

    private static Task SeedUsersAsync(ReferenceDbContext dbContext, CancellationToken cancellationToken)
    {
        var faker =
            new Faker<AuthenticatedUser>().UseSeed(123)
                .CustomInstantiator(
                    f =>
                        new AuthenticatedUser(
                            0,
                            f.Name.FirstName(),
                            f.Make(f.Random.Int(0, 2), _ => f.Name.LastName()).ToArray(),
                            f.Name.LastName()));

        foreach (var user in faker.GenerateLazy(500))
        {
            dbContext.Add(user);
        }

        return Task.CompletedTask;
    }
}