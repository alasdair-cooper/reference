namespace AlasdairCooper.Reference.Shared;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class => source.Where(x => x is not null)!;
}