namespace AlasdairCooper.Reference.Api.Features.Content;

internal sealed record FileUploadQueueStatus(int Count, UploadedMediaLog[] RecentlyUploaded);