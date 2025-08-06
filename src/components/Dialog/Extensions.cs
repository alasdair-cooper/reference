using Microsoft.Extensions.DependencyInjection;

namespace AlasdairCooper.Reference.Components.Dialog;

public static class Extensions
{
    public static IServiceCollection AddDialogs(this IServiceCollection services)
    {
        services.AddScoped<DialogService>();
        return services;
    }
    
    public static IServiceCollection AddDialogsSingleton(this IServiceCollection services)
    {
        services.AddSingleton<DialogService>();
        return services;
    }
}