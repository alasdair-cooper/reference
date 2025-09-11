using System.Threading.Channels;
using AlasdairCooper.Reference.Api.Data;

namespace AlasdairCooper.Reference.Api.Features.Content;

internal sealed class FileUploadWorker(
    Channel<PendingMediaUpload> fileUploadQueue,
    Channel<UploadedMediaLog> logQueue,
    TimeProvider timeProvider,
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var pendingUpload in fileUploadQueue.Reader.ReadAllAsync(stoppingToken))
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            await using var context = scope.ServiceProvider.GetRequiredService<ReferenceDbContext>();
            
            context.Files.Add(new Data.Entities.Content.Media(0, pendingUpload.File.Name, pendingUpload.File.Type, pendingUpload.File.Data.ToArray()));
            await context.SaveChangesAsync(stoppingToken);

            await logQueue.Writer.WriteAsync(
                new UploadedMediaLog(pendingUpload.File.Name, pendingUpload.File.Data.Length, pendingUpload.EnqueuedAt, timeProvider.GetUtcNow()),
                stoppingToken);
        }
    }
}