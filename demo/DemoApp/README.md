# DemoApp — Maui.CredentialManager

A .NET MAUI Blazor app demonstrating the [`Maui.CredentialManager`](../../src/Maui.CredentialManager) library.
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
    // options.AppleServiceId = "your-apple-service-id";
    // options.AppleRedirectUri = "your-apple-redirect-uri";

    // options.Android.GoogleRedirectUri = "your-android-google-redirect-uri";
    // options.Android.GoogleCallbackScheme = "your-android-google-callback-scheme";
    // options.Android.AppleCallbackScheme = "your-android-apple-callback-scheme";

    // options.Ios.GoogleClientId = "your-google-ios-client-id";
    // options.Ios.GoogleRedirectUri = "your-ios-google-redirect-uri";
    // options.Ios.AppleCallbackScheme = "your-ios-apple-callback-scheme";
});
```

### Shared options

| Option | Description |
|--------|-------------|
| `GoogleServerClientId` | OAuth 2.0 **Web** client ID from Google Cloud Console. This is the client ID your backend uses to verify tokens. Required for native Android sign-in and the Google ID option in `GetPasswordCredential`. |
| `AppleServiceId` | Service ID from the Apple Developer portal. |
| `AppleRedirectUri` | Redirect URI registered with Apple. |

### Android options (`options.Android.*`)

| Option | Description |
|--------|-------------|
| `GoogleRedirectUri` | Redirect URI for Android browser-based Google auth. |
| `GoogleCallbackScheme` | Custom URL scheme for Android browser callback. |
| `AppleCallbackScheme` | Callback scheme for Apple auth on Android. |
| `GoogleAuthMethod` | How to handle Google SSO on Android. Default: `Native`. |
| `AppleAuthMethod` | How to handle Apple SSO on Android. Default: `Browser`. |

### iOS options (`options.Ios.*`)

| Option | Description |
|--------|-------------|
| `GoogleClientId` | iOS-specific Google OAuth client ID (for browser flow on iOS). |
| `GoogleRedirectUri` | Redirect URI for iOS browser-based Google auth. |
| `AppleCallbackScheme` | Callback scheme for Apple auth on iOS. |
| `GoogleAuthMethod` | How to handle Google SSO on iOS. Default: `Browser`. |
| `AppleAuthMethod` | How to handle Apple SSO on iOS. Default: `Native`. |

### Auth method defaults

| Provider | Android | iOS |
|----------|---------|-----|
| Google | Native | Browser |
| Apple | Browser | Native |

Override per-platform via `Android.GoogleAuthMethod`, `Android.AppleAuthMethod`, `Ios.GoogleAuthMethod`, `Ios.AppleAuthMethod`. Accepted values: `Native`, `Browser`.

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
