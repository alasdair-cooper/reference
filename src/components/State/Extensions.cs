using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AlasdairCooper.Reference.Components.State;

public static class Extensions
{
    public static OptionsBuilder<StateOptions> AddState(this IServiceCollection services)
    {
        services.AddCascadingAuthenticationState();

        services.AddSingleton<StateStoreFactory>();

        return services.AddOptions<StateOptions>().ValidateOnStart();
    }
}