using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Features.Media;

internal sealed record PendingMediaUpload(BinaryFile File, DateTimeOffset EnqueuedAt);