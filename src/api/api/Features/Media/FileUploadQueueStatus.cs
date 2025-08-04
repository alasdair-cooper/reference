namespace AlasdairCooper.Reference.Api.Features.Media;

internal sealed record FileUploadQueueStatus(int Count, UploadedMediaLog[] RecentlyUploaded);