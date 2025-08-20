using System.Diagnostics;
using System.Text.RegularExpressions;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Data.Entities.Discounts;
using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies.Discounters;
using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Api.Features.Content;
using AlasdairCooper.Reference.Api.Utilities;
using AlasdairCooper.Reference.Shared;
using AlasdairCooper.Reference.Shared.Api;
using AlasdairCooper.Reference.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace AlasdairCooper.Reference.Api.Features.Items;

public static class ItemsApi
{
    public static IEndpointRouteBuilder MapItemsEndpoints(this IEndpointRouteBuilder builder)
    {
        var items = builder.MapGroup("/items").WithTags("Items");

        items.MapGet(
            "/",
            static async (
                int page,
                int pageSize,
                string? nameFilter,
                bool? includeOutOfStock,
                ItemSortableField? sortField,
                SortDirection? sortDirection,
                ReferenceDbContext context,
                HttpResponse httpResponse,
                TimeProvider timeProvider,
                LinkGenerator linkGenerator,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
                ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

                var skus =
                    context.Skus.Where(x => nameFilter == null || Regex.IsMatch(x.DisplayName, nameFilter))
                        .Where(x => (includeOutOfStock != null && includeOutOfStock.Value) || x.Bins.SelectMany(static x => x.Stock).Any());

                var totalCount = await skus.CountAsync(cancellationToken);

                httpResponse.Headers.Append("X-Total-Count", totalCount.ToString());

                return TypedResults.Ok(await GetItems().ToListAsync(cancellationToken));

                async IAsyncEnumerable<ItemDto> GetItems()
                {
                    var pagedSkus =
                        await skus.OrderBy(
                                sortField switch
                                {
                                    ItemSortableField.DisplayName or null => static x => x.DisplayName,
                                    ItemSortableField.SuggestedPrice => static x => x.SuggestedPrice,
                                    _ => throw new UnreachableException()
                                },
                                sortDirection ?? SortDirection.Ascending)
                            .Select(
                                static x =>
                                new
                                {
                                    x.Id,
                                    x.DisplayName,
                                    x.Description,
                                    x.SuggestedPrice,
                                    x.KeyPoints,
                                    x.Tags,
                                    x.Media,
                                    StockCount = x.Bins.SelectMany(static x => x.Stock).Count()
                                })
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    var currentDate = timeProvider.GetUtcNow();

                    var skuIds = pagedSkus.Select(static x => x.Id).ToList();

                    var skuDiscounts =
                        await context.Promotions.Where(x => x.StartDate == null || x.StartDate < currentDate)
                            .Where(x => x.EndDate == null || currentDate < x.EndDate)
                            .SelectMany(static x => x.Discounts)
                            .OfType<SkuDiscount>()
                            .Include(static x => x.Strategy)
                            .Include(x => x.Skus.Where(x => skuIds.Contains(x.Id)))
                            .ToListAsync(cancellationToken);

                    var tagDiscounts =
                        await context.Promotions.Where(x => x.StartDate == null || x.StartDate < currentDate)
                            .Where(x => x.EndDate == null || currentDate < x.EndDate)
                            .SelectMany(static x => x.Discounts)
                            .OfType<TagDiscount>()
                            .Include(static x => x.Strategy)
                            .Include(static x => x.Tags)
                            .ThenInclude(x => x.Skus.Where(x => skuIds.Contains(x.Id)))
                            .ToListAsync(cancellationToken);

                    foreach (var sku in pagedSkus)
                    {
                        var itemBasket = new List<PricedItem> { new(sku.Id, sku.SuggestedPrice) };

                        var discounts =
                            skuDiscounts.Where(x => x.Skus.Any(x => x.Id == sku.Id))
                                .Concat<Discount>(tagDiscounts.Where(x => x.Tags.Any(x => x.Skus.Any(x => x.Id == sku.Id))))
                                .ToList();

                        var discountedPrice =
                            discounts.SelectMany(x => x.Strategy.CalculateDiscount(itemBasket, new DiscountContext()))
                                .OfType<ItemDiscounter>()
                                .Min(x => x.ApplyDiscount(sku.SuggestedPrice));

                        yield return new ItemDto(
                            sku.Id,
                            sku.DisplayName,
                            sku.Description,
                            sku.KeyPoints,
                            discountedPrice ?? sku.SuggestedPrice,
                            sku.SuggestedPrice,
                            sku.StockCount < 10 ? sku.StockCount : null,
                            discounts.Select(static x => x.Strategy.ToDisplayString()).ToArray(),
                            sku.Media.Select(x => linkGenerator.GetUriByName(httpContext, MediaConstants.EndpointNames.GetMedia, new { x.Id }))
                                .WhereNotNull()
                                .ToArray(),
                            sku.Tags.Select(static x => x.Name).ToArray());
                    }
                }
            }).CacheOutput();

        return builder;
    }
}