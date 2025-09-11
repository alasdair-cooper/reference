using System.Text.RegularExpressions;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Data.Entities.Users;
using AlasdairCooper.Reference.Shared.Api;
using Microsoft.EntityFrameworkCore;

namespace AlasdairCooper.Reference.Api.Features.Users;

public static class UsersApi
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        var users = builder.MapGroup("/users").WithTags("Users");

        users.MapGet(
            "/",
            async (int page, int pageSize, string? nameFilter, ReferenceDbContext context, HttpResponse httpResponse, CancellationToken cancellationToken) =>
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
                ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

                var users =
                    await context.Users.OfType<AuthenticatedUser>()
                        .Where(
                            x =>
                                nameFilter == null
                                || Regex.IsMatch(x.FirstName + " " + string.Join(" ", x.MiddleNames) + " " + x.LastName, nameFilter))
                        .OrderBy(x => x.LastName)
                        .Select(x => new UserDto(x.Id, x.FirstName, x.MiddleNames, x.LastName))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync(cancellationToken);
                
                var usersCount = await context.Users.OfType<AuthenticatedUser>()
                    .Where(
                        x =>
                            nameFilter == null
                            || Regex.IsMatch(x.FirstName + " " + string.Join(" ", x.MiddleNames) + " " + x.LastName, nameFilter))
                    .CountAsync(cancellationToken);
                
                httpResponse.Headers.Append("X-Total-Count", usersCount.ToString());

                return TypedResults.Ok(users);
            });

        return builder;
    }
}