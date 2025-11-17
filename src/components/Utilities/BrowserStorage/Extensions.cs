using Microsoft.Extensions.DependencyInjection;

namespace AlasdairCooper.Reference.Components.Utilities.BrowserStorage;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBrowserStorageServices()
        {
            if (OperatingSystem.IsBrowser())
            {
                services.AddSingleton<LocalStorageService>();
                services.AddSingleton<SessionStorageService>();
            }
            else
            {
                services.AddScoped<LocalStorageService>();
                services.AddScoped<SessionStorageService>();
            }

            return services;
        }
    }
}