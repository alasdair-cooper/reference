using System.Threading.Channels;

namespace AlasdairCooper.Reference.Api.Features.Media;

internal static class Extensions
{
    public static IServiceCollection AddMedia(this IServiceCollection services)
    {
        services.AddSingleton(_ => Channel.CreateUnbounded<PendingMediaUpload>());

        services.AddSingleton(
            _ => Channel.CreateBounded<UploadedMediaLog>(new BoundedChannelOptions(10) { FullMode = BoundedChannelFullMode.DropOldest }));

        services.AddHostedService<FileUploadWorker>();

        return services;
    }
}