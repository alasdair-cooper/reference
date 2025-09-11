using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Features.Content;

internal sealed record PendingMediaUpload(BinaryFile File, DateTimeOffset EnqueuedAt);