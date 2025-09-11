using System.Diagnostics;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Components.Tables;

public static class Extensions
{
    public static IOrderedEnumerable<TSource> OrderByDirection<TSource>(
        this IEnumerable<TSource> source,
        SortDirection direction,
        Func<TSource, object?> keySelector) =>
        direction switch
        {
            SortDirection.Ascending => source.OrderBy(keySelector),
            SortDirection.Descending => source.OrderByDescending(keySelector),
            _ => throw new UnreachableException()
        };
}