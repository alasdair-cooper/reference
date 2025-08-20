namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public interface ILocatable
{
    public ILocatable? Parent { get; }

    public string Key { get; }
}

public static class LocatableExtensions
{
    public static string GetKey(this ILocatable locatable) =>
        locatable.Parent is null ? locatable.Key : $"{locatable.Parent.GetKey()}->{locatable.Key}";
}