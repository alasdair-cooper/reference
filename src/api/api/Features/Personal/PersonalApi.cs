using System.Security.Claims;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Data.Entities.Users;
using AlasdairCooper.Reference.Api.Features.Content;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AlasdairCooper.Reference.Api.Features.Personal;

public static class PersonalApi
{
    public static IEndpointRouteBuilder MapPersonalEndpoints(this IEndpointRouteBuilder builder)
    {
        var me = builder.MapGroup("/me").WithTags("Personal");

        me.MapPost(
                "/login",
                async static (
                    ReferenceDbContext dbContext,
                    CancellationToken cancellationToken) =>
                {
                    var user = new AnonymousUser(0, Guid.NewGuid().ToString());
                    dbContext.Add(user);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    return Results.SignIn(
                        new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, user.CorrelationId)], null)),
                        authenticationScheme: CookieAuthenticationDefaults.AuthenticationScheme);
                })
            .RequireAuthorization(static x => x.RequireAssertion(static x => !x.User.HasClaim(static x => x.Type == ClaimTypes.NameIdentifier)));

        me.MapGet(
            "/claims",
            static (ClaimsPrincipal user) => Results.Ok(user.Claims.Select(static x => new { x.Type, x.Value })));

        me.MapGet("/basket", static (HttpContext httpContext) => Results.Ok(httpContext.Session.Basket.Items));

        me.MapPost(
                "/basket",
                static async (
                    AddItemsToBasketRequest request,
                    ReferenceDbContext dbContext,
                    HttpContext httpContext,
                    LinkGenerator linkGenerator,
                    ClaimsPrincipal user,
                    IHubContext<PersonalHub, IPersonalClient> hubContext) =>
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

                    await hubContext.Clients.User(user.NameIdentifier!).BasketUpdated();

                    return Results.Created();
                })
            .RequireAuthorization(static x => x.RequireClaim(ClaimTypes.NameIdentifier));

        me.MapPut(
                "/basket/{skuId:int}",
                static async (
                    int skuId,
                    UpdateItemsInBasketRequest request,
                    HttpContext httpContext,
                    ClaimsPrincipal user,
                    IHubContext<PersonalHub, IPersonalClient> hubContext) =>
                {
                    httpContext.Session.Basket.TryUpdateItem(skuId, x => x with { Count = request.Count });
                    await hubContext.Clients.User(user.NameIdentifier!).BasketUpdated();
                    return Results.NoContent();
                })
            .RequireAuthorization(static x => x.RequireClaim(ClaimTypes.NameIdentifier));

        me.MapDelete(
                "/basket/{skuId:int}",
                static async (int skuId, HttpContext httpContext, ClaimsPrincipal user, IHubContext<PersonalHub, IPersonalClient> hubContext) =>
                {
                    httpContext.Session.Basket.TryRemoveItem(skuId);
                    await hubContext.Clients.User(user.NameIdentifier!).BasketUpdated();
                    return Results.NoContent();
                })
            .RequireAuthorization(static x => x.RequireClaim(ClaimTypes.NameIdentifier));

        return builder;
    }
}

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal user)
    {
        public string? NameIdentifier => user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}