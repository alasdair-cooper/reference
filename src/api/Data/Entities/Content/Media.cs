using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Content;

public sealed class Media(int id, string name, string mediaType, byte[] data) : File(id, name, data)
{
    [StringLength(300)]
    public string MediaType { get; init; } = mediaType;
}