using Maui.CredentialManagers.Extensions;
using Microsoft.Extensions.Logging;

namespace DemoApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddCredentialManagerService(options =>
        {
            // Configure SSO client IDs here when testing SSO flows
            // options.GoogleServerClientId = "your-google-client-id";
            // options.GoogleIosClientId = "your-google-ios-client-id";
            // options.GoogleIosRedirectUri = "your-redirect-uri";
            // options.AppleServiceId = "your-apple-service-id";
            // options.AppleRedirectUri = "your-apple-redirect-uri";
        });

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
