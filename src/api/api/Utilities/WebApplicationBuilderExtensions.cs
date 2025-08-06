namespace AlasdairCooper.Reference.Api.Utilities;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddCorsForClient(this WebApplicationBuilder builder, string resourceName)
    {
        builder.Services.AddCors(
            x =>
                x.AddDefaultPolicy(
                    x =>
                        x.WithOrigins(builder.Configuration.GetServiceEndpoints(resourceName)).AllowAnyMethod().WithExposedHeaders("X-Total-Count")));

        return builder;
    }
}