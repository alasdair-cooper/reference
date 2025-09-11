using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Shared.Api;

namespace AlasdairCooper.Reference.Api.Features.Discounts;

internal static class DiscountsApi
{
    public static IEndpointRouteBuilder MapDiscountsEndpoints(this IEndpointRouteBuilder builder)
    {
        var discounts = builder.MapGroup("/discounts").WithTags("Discounts");

        var strategies = discounts.MapGroup("/strategies");

        strategies.MapGet(
            "/",
            (ReferenceDbContext context) =>
            {
                var strategies = context.DiscountStrategies.AsEnumerable().Select(x => new DiscountStrategyDto(x.Id, x.ToDisplayString())).ToList();
                return Results.Ok(strategies);
            });

        return builder;
    }
}