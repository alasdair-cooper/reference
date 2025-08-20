using System.Threading.Channels;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AlasdairCooper.Reference.Api.Features.Content;

internal static class ContentApi
{
    public static IEndpointRouteBuilder MapContentEndpoints(this IEndpointRouteBuilder builder)
    {
        var content = builder.MapGroup("/content").WithTags("Content");

        content.MapGet(
                "/{id:int}",
                async (int id, ReferenceDbContext context, CancellationToken cancellationToken) =>
                {
                    var media =
                        await context.Files.OfType<Data.Entities.Content.Media>()
                            .Where(x => x.Id == id)
                            .Select(x => new { x.MediaType, x.Data })
                            .SingleOrDefaultAsync(cancellationToken);

                    return media is not null ? Results.File(media.Data, media.MediaType) : Results.NotFound();
                })
            .WithName(MediaConstants.EndpointNames.GetMedia);

        content.MapPost(
                "/",
                async (
                    IFormFileCollection files,
                    Channel<PendingMediaUpload> fileUploadQueue,
                    TimeProvider timeProvider,
                    LinkGenerator linkGenerator,
                    HttpContext httpContext) =>
                {
                    await foreach (var file in files.ToAsyncEnumerable().Select(async (x, y) => await BinaryFileHelpers.FromFormFileAsync(x, y)))
                    {
                        await fileUploadQueue.Writer.WriteAsync(new PendingMediaUpload(file, timeProvider.GetUtcNow()));
                    }

                    return Results.Accepted(linkGenerator.GetUriByName(httpContext, MediaConstants.EndpointNames.GetUploadQueueStatus));
                })
            .DisableAntiforgery();

        content.MapGet(
                "/upload-queue/status",
                (Channel<PendingMediaUpload> fileUploadQueue, Channel<UploadedMediaLog> logQueue) =>
                    Results.Ok(new FileUploadQueueStatus(fileUploadQueue.Reader.Count, logQueue.Reader.TryPeek(out var log) ? [log] : [])))
            .WithName(MediaConstants.EndpointNames.GetUploadQueueStatus);

        return builder;
    }
}