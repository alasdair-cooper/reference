namespace AlasdairCooper.Reference.Api.Utilities;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureJson(this IServiceCollection services) =>
        services.ConfigureHttpJsonOptions(static x => x.SerializerOptions.Converters.AddDefaultConverters());
}