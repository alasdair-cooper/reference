namespace AlasdairCooper.Reference.Api.Utilities;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddCorsForClient(this WebApplicationBuilder builder, string connectionName)
    {
        builder.Services.AddCors(
            x =>
                x.AddDefaultPolicy(
                    x =>
                        x.WithOrigins(builder.Configuration.GetServiceEndpoints(connectionName))
                            .WithOrigins("http://localhost:8080")
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithHeaders("content-type")
                            .WithExposedHeaders("X-Total-Count", "content-type")));

        return builder;
    }
}