using System.Diagnostics;
using System.Linq.Expressions;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Utilities;

public static class QueryableExtensions
{
    public static IOrderedQueryable<TSource> OrderBy<TSource>(
        this IQueryable<TSource> source,
        Expression<Func<TSource, object?>> keySelector,
        SortDirection direction) =>
        direction switch
        {
            SortDirection.Ascending => source.OrderBy(keySelector),
            SortDirection.Descending => source.OrderByDescending(keySelector),
            _ => throw new UnreachableException()
        };
}