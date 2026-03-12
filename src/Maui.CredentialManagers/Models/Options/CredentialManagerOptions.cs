namespace Maui.CredentialManagers.Models.Options;

/// <summary>
/// Configuration for the credential manager, including shared SSO settings
/// and platform-specific options for Android and iOS.
/// </summary>
public class CredentialManagerOptions
{
    /// <summary>
    /// OAuth 2.0 Web client ID from Google Cloud Console.
    /// Used by the backend to verify tokens and required for native Android sign-in.
    /// </summary>
    public string? GoogleServerClientId { get; set; }

    /// <summary>
    /// Service ID registered in the Apple Developer portal for Sign in with Apple.
    /// </summary>
    public string? AppleServiceId { get; set; }

    /// <summary>
    /// Redirect URI registered with Apple for Sign in with Apple.
    /// </summary>
    public string? AppleRedirectUri { get; set; }

    /// <summary>
    /// Android-specific credential options.
    /// </summary>
    public AndroidCredentialOptions Android { get; set; } = new();

    /// <summary>
    /// iOS-specific credential options.
    /// </summary>
    public IosCredentialOptions Ios { get; set; } = new();
}

/// <summary>
/// Android-specific SSO configuration.
/// </summary>
public class AndroidCredentialOptions
{
    /// <summary>
    /// Redirect URI for browser-based Google authentication on Android.
    /// </summary>
    public string? GoogleRedirectUri { get; set; }

    /// <summary>
    /// Custom URL scheme used to capture the browser callback for Google auth on Android.
    /// </summary>
    public string? GoogleCallbackScheme { get; set; }

    /// <summary>
    /// Custom URL scheme used to capture the browser callback for Apple auth on Android.
    /// </summary>
    public string? AppleCallbackScheme { get; set; }

    /// <summary>
    /// How Google SSO is handled on Android. Defaults to <see cref="SsoAuthMethod.Native"/>.
    /// </summary>
    public SsoAuthMethod GoogleAuthMethod { get; set; } = SsoAuthMethod.Native;

    /// <summary>
    /// How Apple SSO is handled on Android. Defaults to <see cref="SsoAuthMethod.Browser"/>.
    /// Native Apple Sign-In is not supported on Android.
    /// </summary>
    public SsoAuthMethod AppleAuthMethod { get; } = SsoAuthMethod.Browser;
}

/// <summary>
/// iOS-specific SSO configuration.
/// </summary>
public class IosCredentialOptions
{
    /// <summary>
    /// iOS-specific Google OAuth client ID for browser-based Google authentication.
    /// </summary>
    public string? GoogleClientId { get; set; }

    /// <summary>
    /// Redirect URI for browser-based Google authentication on iOS.
    /// </summary>
    public string? GoogleRedirectUri { get; set; }

    /// <summary>
    /// Custom URL scheme used to capture the browser callback for Apple auth on iOS.
    /// </summary>
    public string? AppleCallbackScheme { get; set; }

    /// <summary>
    /// How Google SSO is handled on iOS. Defaults to <see cref="SsoAuthMethod.Browser"/>.
    /// Native Google Sign-In is not supported on iOS.
    /// </summary>
    public SsoAuthMethod GoogleAuthMethod { get; } = SsoAuthMethod.Browser;

    /// <summary>
    /// How Apple SSO is handled on iOS. Defaults to <see cref="SsoAuthMethod.Native"/>.
    /// </summary>
    public SsoAuthMethod AppleAuthMethod { get; set; } = SsoAuthMethod.Native;
}
