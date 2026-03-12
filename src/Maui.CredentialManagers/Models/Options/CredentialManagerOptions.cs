namespace Maui.CredentialManagers.Models.Options;

public class CredentialManagerOptions
{
    // Google SSO
    public string? GoogleServerClientId { get; set; }
    public string? GoogleIosClientId { get; set; }
    public string? GoogleIosRedirectUri { get; set; }
    public string? GoogleAndroidRedirectUri { get; set; }
    public string? GoogleAndroidCallbackScheme { get; set; }

    // Apple SSO
    public string? AppleServiceId { get; set; }
    public string? AppleRedirectUri { get; set; }
    public string? AppleAndroidCallbackScheme { get; set; }
    public string? AppleIosCallbackScheme { get; set; }

    // SSO auth method per provider per platform (defaults match current behavior)
    public SsoAuthMethod GoogleOnAndroid { get; set; } = SsoAuthMethod.Native;
    public SsoAuthMethod AppleOnAndroid { get; set; } = SsoAuthMethod.Browser;
    public SsoAuthMethod GoogleOnIos { get; set; } = SsoAuthMethod.Browser;
    public SsoAuthMethod AppleOnIos { get; set; } = SsoAuthMethod.Native;
}
