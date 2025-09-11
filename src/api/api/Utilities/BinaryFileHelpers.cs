using AlasdairCooper.Reference.Shared.Common;
using Microsoft.AspNetCore.Components.Forms;

namespace AlasdairCooper.Reference.Api.Utilities;

public static class BinaryFileHelpers
{
    public static async Task<BinaryFile> FromBrowserFileAsync(
        IBrowserFile file,
        long maxAllowedSizeBytes = 512_000L,
        CancellationToken cancellationToken = default)
    {
        await using var stream = file.OpenReadStream(maxAllowedSizeBytes, cancellationToken);
        return new BinaryFile(file.Name, file.ContentType, await BinaryData.FromStreamAsync(stream, cancellationToken));
    }

    public static async Task<BinaryFile> FromFormFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        await using var stream = file.OpenReadStream();
        return new BinaryFile(file.FileName, file.ContentType, await BinaryData.FromStreamAsync(stream, cancellationToken));
    }
}