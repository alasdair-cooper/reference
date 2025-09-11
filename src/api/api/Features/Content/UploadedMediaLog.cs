namespace AlasdairCooper.Reference.Api.Features.Content;

internal sealed record UploadedMediaLog(string FileName, long FileSize, DateTimeOffset EnqueuedAt, DateTimeOffset UploadedAt);