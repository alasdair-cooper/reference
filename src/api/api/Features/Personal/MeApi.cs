using System.Text.Json;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Features.Content;
using AlasdairCooper.Reference.Api.Utilities;
using AlasdairCooper.Reference.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace AlasdairCooper.Reference.Api.Features.Personal;

public static class PersonalApi
{
    public static IEndpointRouteBuilder MapPersonalEndpoints(this IEndpointRouteBuilder builder)
    {
        var me = builder.MapGroup("/me").WithTags("Personal");

        me.MapGet("/basket", static (HttpContext httpContext) => Results.Ok(httpContext.Session.Basket.Items));

        me.MapPost(
            "/basket",
            static async (AddItemsToBasketRequest request, ReferenceDbContext dbContext, HttpContext httpContext, LinkGenerator linkGenerator) =>
            {
                var sku =
                    await dbContext.Skus.Where(x => x.Id == request.SkuId)
                        .Select(static x => new { x.Id, x.SuggestedPrice, x.Media, x.DisplayName })
                        .SingleOrDefaultAsync();

                if (sku is null)
                {
                    return Results.NotFound();
                }

                httpContext.Session.Basket.AddItem(
                    new BasketItemSnapshot(
                        sku.Id,
                        sku.DisplayName,
                        sku.Media.Select(x => linkGenerator.GetUriByName(httpContext, MediaConstants.EndpointNames.GetMedia, new { x.Id }))
                            .FirstOrDefault(),
                        sku.SuggestedPrice,
                        sku.SuggestedPrice,
                        request.Count));

                return Results.Created();
            });

        me.MapPut(
            "/basket/{skuId:int}",
            static (int skuId, UpdateItemsInBasketRequest request, HttpContext httpContext) =>
            {
                httpContext.Session.Basket.TryUpdateItem(skuId, x => x with { Count = request.Count });
                return Results.NoContent();
            });

        me.MapDelete(
            "/basket/{skuId:int}",
            static (int skuId, HttpContext httpContext) =>
            {
                httpContext.Session.Basket.TryRemoveItem(skuId);
                return Results.NoContent();
            });

        return builder;
    }
}

public static class SessionExtensions
{
    extension(ISession session)
    {
        public BasketStore Basket => new(session);

        public T? Get<T>(string key) =>
            session.GetString(key) is { } val && JsonSerializer.Deserialize<T>(val, JsonSerializerOptions.Session) is { } res ? res : default;

        public void Set<T>(string key, T value) => session.SetString(key, JsonSerializer.Serialize(value, JsonSerializerOptions.Session));
    }
}

public class BasketStore(ISession session)
{
    public IEnumerable<BasketItemSnapshot> Items => session.Get<List<BasketItemSnapshot>>("basket") ?? [];

    public void AddItem(BasketItemSnapshot item)
    {
        var existingItems = Items.ToList();

        if (existingItems.FirstOrDefault(x => x.SkuId == item.SkuId) is { } existingItem)
        {
            existingItems.Remove(existingItem);
            existingItems.Add(existingItem with { Count = existingItem.Count + item.Count });
        }
        else
        {
            existingItems.Add(item);
        }

        session.Set("basket", existingItems);
    }

    public void TryUpdateItem(int skuId, Func<BasketItemSnapshot, BasketItemSnapshot> update)
    {
        var existingItems = Items.ToList();

        if (existingItems.FirstOrDefault(x => x.SkuId == skuId) is not { } existingItem) return;

        existingItems.Remove(existingItem);
        var updatedItem = update(existingItem);
        existingItems.Add(updatedItem);
        session.Set("basket", existingItems);
    }

    public void TryRemoveItem(int skuId)
    {
        var existingItems = Items.ToList();

        if (existingItems.FirstOrDefault(x => x.SkuId == skuId) is not { } existingItem) return;

        existingItems.Remove(existingItem);
        session.Set("basket", existingItems);
    }
}

public record BasketItemSnapshot(int SkuId, string DisplayName, string? ThumbnailUrl, Money? OriginalPrice, Money ActualPrice, int Count);

public record AddItemsToBasketRequest(int SkuId, int Count);

public record UpdateItemsInBasketRequest(int Count);