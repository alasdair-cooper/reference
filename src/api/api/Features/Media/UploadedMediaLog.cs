namespace AlasdairCooper.Reference.Api.Features.Media;

internal sealed record UploadedMediaLog(string FileName, long FileSize, DateTimeOffset EnqueuedAt, DateTimeOffset UploadedAt);