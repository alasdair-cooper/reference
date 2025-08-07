using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;

namespace AlasdairCooper.Reference.Components.Inputs;

public static class Extensions
{
    public static IServiceCollection AddInputs(this IServiceCollection services)
    {
        services.AddBlazoredLocalStorage();

        return services;
    }
    
    public static IServiceCollection AddInputsAsSingleton(this IServiceCollection services)
    {
        services.AddBlazoredLocalStorageAsSingleton();

        return services;
    }
}