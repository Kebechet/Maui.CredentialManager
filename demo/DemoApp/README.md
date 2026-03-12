# DemoApp — Maui.CredentialManagers

A .NET MAUI Blazor app demonstrating the [`Maui.CredentialManagers`](../../src/Maui.CredentialManagers) library.
Targets `net10.0-android` and `net10.0-ios`.

## Features demonstrated

- Save password credentials (Credential Manager on Android, Keychain on iOS)
- Retrieve password credentials (with optional auto-select and filtering)
- SSO login via Google, Apple, or PlatformDefault (Google on Android, Apple on iOS)
- Clear credential state

## Prerequisites

- .NET 10 SDK
- MAUI workload (`dotnet workload install maui`)
- **Android**: Android SDK with API 24+
- **iOS**: Xcode with iOS 16.0+ SDK

## Configuration

All SSO options are set in [`MauiProgram.cs`](MauiProgram.cs) via `AddCredentialManagerService`:

```csharp
builder.Services.AddCredentialManagerService(options =>
{
    options.GoogleServerClientId = "your-google-web-client-id";
    // options.GoogleIosClientId = "your-google-ios-client-id";
    // options.GoogleIosRedirectUri = "your-redirect-uri";
    // options.AppleServiceId = "your-apple-service-id";
    // options.AppleRedirectUri = "your-apple-redirect-uri";
});
```

### Google SSO

| Option | Description |
|--------|-------------|
| `GoogleServerClientId` | OAuth 2.0 **Web** client ID from Google Cloud Console. This is the client ID your backend uses to verify tokens. Required for native Android sign-in and the Google ID option in `GetPasswordCredential`. |
| `GoogleIosClientId` | iOS-specific Google OAuth client ID (for browser flow on iOS). |
| `GoogleIosRedirectUri` | Redirect URI for iOS browser-based Google auth. |
| `GoogleAndroidRedirectUri` | Redirect URI for Android browser-based Google auth. |
| `GoogleAndroidCallbackScheme` | Custom URL scheme for Android browser callback. |

### Apple SSO

| Option | Description |
|--------|-------------|
| `AppleServiceId` | Service ID from the Apple Developer portal. |
| `AppleRedirectUri` | Redirect URI registered with Apple. |
| `AppleAndroidCallbackScheme` | Callback scheme for Apple auth on Android. |
| `AppleIosCallbackScheme` | Callback scheme for Apple auth on iOS. |

### Auth method defaults

| Provider | Android | iOS |
|----------|---------|-----|
| Google | Native | Browser |
| Apple | Browser | Native |

Override per-platform via `GoogleOnAndroid`, `AppleOnAndroid`, `GoogleOnIos`, `AppleOnIos`. Accepted values: `Native`, `Browser`, `Disabled`.

## Password credentials — no SSO setup needed

Saving and retrieving password credentials works without any SSO configuration. You can test `CreatePasswordCredential`, `GetPasswordCredential`, and `ClearCredentialState` immediately.

## Running

```bash
# Android
dotnet build -f net10.0-android

# iOS
dotnet build -f net10.0-ios
```

## Android-specific notes

- **⚠️ The `<ApplicationId>` in your `.csproj` must exactly match the package name registered in the Google Cloud Console for your OAuth client.** If they don't match, credential requests will silently fail or return empty results with no clear error message. The demo app uses `com.kebechet.demoapp` — make sure the same package name is configured in your Google Cloud project.
- Requires `INTERNET` and `ACCESS_NETWORK_STATE` permissions (already in manifest).
- For passkeys: set up an `assetlinks.json` on your domain and configure Digital Asset Links.

## iOS-specific notes

- Minimum deployment target: iOS 16.0 (required for ASAuthorization passkey APIs).
- For Apple Sign In: enable the **Sign In with Apple** capability in the Apple Developer portal.
- For passkeys: configure **Associated Domains** in your entitlements.
