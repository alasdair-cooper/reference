using System.Diagnostics;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Data.Entities;
using AlasdairCooper.Reference.Api.Data.Entities.Discounts;
using AlasdairCooper.Reference.Api.Data.Entities.DiscountStrategies;
using AlasdairCooper.Reference.Api.Features.Content;
using AlasdairCooper.Reference.Shared;
using AlasdairCooper.Reference.Shared.Api;
using Microsoft.EntityFrameworkCore;

namespace AlasdairCooper.Reference.Api.Features.Promotions;

internal static class PromotionsApi
{
    public static IEndpointRouteBuilder MapPromotionsEndpoints(this IEndpointRouteBuilder builder)
    {
        var promotions = builder.MapGroup("/promotions").WithTags("Promotions");

        promotions.MapGet(
            "/",
            async (ReferenceDbContext context, LinkGenerator linkGenerator, HttpContext httpContext, CancellationToken cancellationToken) =>
            {
                var promotions =
                    await context.Promotions.Select(
                            x =>
                            new
                            {
                                x.Id,
                                x.Name,
                                x.DisplayName,
                                x.Description,
                                x.StartDate,
                                x.EndDate,
                                x.Media
                            })
                        .ToListAsync(cancellationToken);

                return Results.Ok(
                    promotions.Select(
                            x =>
                                new PromotionDto(
                                    x.Id,
                                    x.Name,
                                    x.DisplayName,
                                    x.Description,
                                    x.StartDate,
                                    x.EndDate,
                                    x.Media.Select(x => linkGenerator.GetUriByName(httpContext, MediaConstants.EndpointNames.GetMedia, new { x.Id }))
                                        .WhereNotNull()
                                        .ToArray()))
                        .ToList());
            });

        promotions.MapPost(
            "/",
            async (CreatePromotionRequest request, ReferenceDbContext context, CancellationToken cancellationToken) =>
            {
                var discounts =
                    request.Discounts.SelectMany(
                        IEnumerable<Discount> (x) =>
                            x switch
                            {
                                { Strategy: var strategy, SkuIds: { Length: > 0 } skuIds, TagIds.Length: 0 } =>
                                [
                                    new SkuDiscount(0)
                                    {
                                        Strategy = GetStrategy(strategy), Skus = context.Skus.Where(x => skuIds.Contains(x.Id)).ToList()
                                    }
                                ],
                                { Strategy: var strategy, SkuIds.Length: 0, TagIds: { Length: > 0 } tagIds } =>
                                [
                                    new TagDiscount(0)
                                    {
                                        Strategy = GetStrategy(strategy), Tags = context.Tags.Where(x => tagIds.Contains(x.Id)).ToList()
                                    }
                                ],
                                { Strategy: var strategy, SkuIds: { Length: > 0 } skuIds, TagIds: { Length: > 0 } tagIds } =>
                                [
                                    new SkuDiscount(0)
                                    {
                                        Strategy = GetStrategy(strategy), Skus = context.Skus.Where(x => skuIds.Contains(x.Id)).ToList()
                                    },
                                    new TagDiscount(0)
                                    {
                                        Strategy = GetStrategy(strategy), Tags = context.Tags.Where(x => tagIds.Contains(x.Id)).ToList()
                                    }
                                ],
                                _ => []
                            });

                var media =
                    await context.Files.OfType<Data.Entities.Content.Media>()
                        .Where(x => request.MediaIds.Contains(x.Id))
                        .ToListAsync(cancellationToken);

                var promotion =
                    new Promotion(0, request.Name, request.DisplayName, request.Description, request.StartDate, request.EndDate)
                    {
                        Discounts = discounts.ToList(),
                        Media = media
                    };

                context.Add(promotion);
                await context.SaveChangesAsync(cancellationToken);

                return TypedResults.CreatedAtRoute(PromotionsConstants.EndpointNames.GetPromotion, new { id = promotion.Id });

                DiscountStrategy GetStrategy(DiscountStrategyDetails details) =>
                    details switch
                    {
                        BuyNForXDiscountStrategyDetails x => new BuyNForXPriceDiscountStrategy(0, x.N, x.X),
                        _ => throw new UnreachableException()
                    };
            });

        promotions.MapGet(
                "/{id:int}",
                async (
                    int id,
                    ReferenceDbContext context,
                    LinkGenerator linkGenerator,
                    HttpContext httpContext,
                    CancellationToken cancellationToken) =>
                {
                    var promotion =
                        await context.Promotions.Where(x => x.Id == id)
                            .Select(
                                x =>
                                new
                                {
                                    x.Id,
                                    x.Name,
                                    x.DisplayName,
                                    x.Description,
                                    x.StartDate,
                                    x.EndDate,
                                    x.Media
                                })
                            .SingleOrDefaultAsync(cancellationToken);

                    return promotion is not null
                        ? Results.Ok(
                            new PromotionDto(
                                promotion.Id,
                                promotion.Name,
                                promotion.DisplayName,
                                promotion.Description,
                                promotion.StartDate,
                                promotion.EndDate,
                                promotion.Media.Select(
                                        x => linkGenerator.GetUriByName(httpContext, MediaConstants.EndpointNames.GetMedia, new { x.Id }))
                                    .WhereNotNull()
                                    .ToArray()))
                        : Results.NotFound();
                })
            .WithName(PromotionsConstants.EndpointNames.GetPromotion);

        promotions.MapGet(
            "/{id:int}/discounts",
            async (int id, ReferenceDbContext context, CancellationToken cancellationToken) =>
            {
                var promotion =
                    await context.Promotions.Where(x => x.Id == id)
                        .Select(
                            x =>
                                new
                                {
                                    Discounts =
                                        x.Discounts.Select(
                                            x =>
                                                new
                                                {
                                                    x.Id,
                                                    x.Strategy,
                                                    SkuIds =
                                                        x is SkuDiscount
                                                            ? (x as SkuDiscount)!.Skus.Select(x => x.Id).ToArray()
                                                            : Array.Empty<int>(),
                                                    TagIds =
                                                        x is TagDiscount
                                                            ? (x as TagDiscount)!.Tags.Select(x => x.Id).ToArray()
                                                            : Array.Empty<int>()
                                                })
                                })
                        .SingleOrDefaultAsync(cancellationToken);

                return promotion is not null
                    ? Results.Ok(promotion.Discounts.Select(x => new DiscountDto(x.Id, x.Strategy.ToDisplayString(), x.SkuIds, x.TagIds)))
                    : Results.NotFound();
            });

        return builder;
    }
}