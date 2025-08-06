using System.Diagnostics;

namespace AlasdairCooper.Reference.Components.Tables;

public static class Extensions
{
    public static IOrderedEnumerable<TSource> OrderByDirection<TSource, TKey>(
        this IEnumerable<TSource> source,
        SortDirection direction,
        Func<TSource, TKey> keySelector) =>
        direction switch
        {
            SortDirection.Ascending => source.OrderBy(keySelector),
            SortDirection.Descending => source.OrderByDescending(keySelector),
            _ => throw new UnreachableException()
        };
}