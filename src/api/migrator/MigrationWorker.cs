using System.Diagnostics;
using AlasdairCooper.Reference.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace AlasdairCooper.Reference.Api.Migrator;

public class MigrationWorker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ReferenceDbContext>();

            await RunMigrationAsync(dbContext, cancellationToken);
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
}