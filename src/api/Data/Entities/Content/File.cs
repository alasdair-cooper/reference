using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Content;

public class File(int id, string name, byte[] data)
{
    public int Id { get; init; } = id;

    [StringLength(300)]
    public string Name { get; init; } = name;

    public byte[] Data { get; init; } = data;
}