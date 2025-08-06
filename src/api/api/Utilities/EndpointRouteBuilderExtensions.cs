using Scalar.AspNetCore;

namespace AlasdairCooper.Reference.Api.Utilities;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapOpenApiWithScalar(this IEndpointRouteBuilder builder)
    {
        builder.MapOpenApi();
        // See https://github.com/dotnet/aspnetcore/issues/57332#issuecomment-2480939916
        builder.MapScalarApiReference(x => x.Servers = []);

        return builder;
    }
}