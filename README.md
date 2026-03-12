[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/kebechet)

# Maui.CredentialManager
[![NuGet Version](https://img.shields.io/nuget/v/Kebechet.Maui.CredentialManager)](https://www.nuget.org/packages/Kebechet.Maui.CredentialManager/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Kebechet.Maui.CredentialManager)](https://www.nuget.org/packages/Kebechet.Maui.CredentialManager/)
[![Build](https://github.com/Kebechet/Maui.CredentialManagers/actions/workflows/build.yml/badge.svg)](https://github.com/Kebechet/Maui.CredentialManagers/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/Kebechet/Maui.CredentialManagers/graph/badge.svg)](https://codecov.io/gh/Kebechet/Maui.CredentialManagers)
![Last updated](https://img.shields.io/github/last-commit/Kebechet/Maui.CredentialManagers/main?label=last%20updated)
[![Twitter](https://img.shields.io/twitter/url/https/twitter.com/samuel_sidor.svg?style=social&label=Follow%20samuel_sidor)](https://x.com/samuel_sidor)

Cross-platform .NET MAUI library for unified credential management on Android and iOS.

## Installation

```bash
dotnet add package Kebechet.Maui.CredentialManager
```

## Usage

### Registration

```csharp
builder.Services.AddCredentialManagerService(options =>
{
    options.GoogleServerClientId = "xxx.apps.googleusercontent.com";
    options.GoogleIosClientId = "yyy.apps.googleusercontent.com";
    options.GoogleIosRedirectUri = "com.myapp:/oauth2redirect";
    options.AppleServiceId = "com.myapp.auth";
    options.AppleRedirectUri = "https://myserver.com/auth/apple/callback";
    options.AppleAndroidCallbackScheme = "com.myapp:/applecallback";
});
```

### Inject and use

```csharp
@inject ICredentialManagerService CredentialManagerService

// Save password
await CredentialManagerService.CreatePasswordCredential(
    new PasswordCredentialDto { Id = "user@example.com", Password = "secret" },
    cancellationToken);

// Retrieve password
var result = await CredentialManagerService.GetPasswordCredential(cancellationToken);

// SSO (Google on Android, Apple on iOS by default)
var ssoResult = await CredentialManagerService.ContinueWithSso(
    SsoProvider.PlatformDefault, cancellationToken);

// Clear credential state
await CredentialManagerService.ClearCredentialState(cancellationToken);
```

## Supported Platforms

| Feature | Android | iOS |
|---------|---------|-----|
| Password credentials | Credential Manager API | Keychain Services |
| Google Sign-In | Native (Credential Manager) | WebAuthenticator |
| Apple Sign-In | WebAuthenticator | Native (ASAuthorization) |
| Clear credential state | ClearCredentialState API | Keychain removal |

## License

[MIT](LICENSE)
