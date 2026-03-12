using Maui.CredentialManagers.Models.Options;
using Maui.CredentialManagers.Services;
#if ANDROID
using Maui.CredentialManagers.Platforms.Android.Services;
#elif IOS
using Maui.CredentialManagers.Platforms.iOS.Services;
#endif

namespace Maui.CredentialManagers.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCredentialManagerService(
        this IServiceCollection services,
        Action<CredentialManagerOptions> configure)
    {
        var options = new CredentialManagerOptions();
        configure(options);

        services.AddSingleton(options);

#if ANDROID
        services.AddScoped<CredentialManagerAndroidService>();
        services.AddScoped<ICredentialManagerService>(provider =>
        {
            var androidService = provider.GetRequiredService<CredentialManagerAndroidService>();
            var opts = provider.GetRequiredService<CredentialManagerOptions>();
            return new CredentialManagerService(androidService, opts);
        });
#elif IOS
        services.AddScoped<CredentialManagerIosService>();
        services.AddScoped<ICredentialManagerService>(provider =>
        {
            var iosService = provider.GetRequiredService<CredentialManagerIosService>();
            var opts = provider.GetRequiredService<CredentialManagerOptions>();
            return new CredentialManagerService(iosService, opts);
        });
#else
        services.AddScoped<ICredentialManagerService>(provider =>
        {
            var opts = provider.GetRequiredService<CredentialManagerOptions>();
            return new CredentialManagerService(opts);
        });
#endif

        return services;
    }
}
