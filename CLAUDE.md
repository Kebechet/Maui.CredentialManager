# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`Kebechet.Maui.CredentialManager` — a cross-platform .NET MAUI library that exposes a single unified API (`ICredentialManagerService`) for password credentials, passkeys, and SSO (Google/Apple), backed by each platform's native credential APIs. Published to NuGet and GitHub Packages.

- Android → `androidx.credentials` Credential Manager + Google Identity
- iOS → Keychain Services + `AuthenticationServices` (ASAuthorization)
- macOS / Windows → stub implementations that always return an `ErrorMessage` ("not supported")

Target frameworks: `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`, plus `net10.0-windows10.0.19041.0` (Windows TFM only added when building on Windows).

## Commands

The library solution is `src/Maui.CredentialManager.slnx`. The demo app is a separate solution `demo/DemoApp.slnx`.

```bash
# Restore + build the library (Release as CI does)
dotnet restore src/Maui.CredentialManager.slnx
dotnet build src/Maui.CredentialManager.slnx --configuration Release

# Build a single TFM (faster than building all targets)
dotnet build src/Maui.CredentialManager/Maui.CredentialManager.csproj -f net10.0-android

# Pack the NuGet package (also happens on build via GeneratePackageOnBuild)
dotnet pack src/Maui.CredentialManager/Maui.CredentialManager.csproj --configuration Release --output ./nupkg

# Prerequisite for any build: the MAUI workload
dotnet workload install maui
```

CI (`.github/workflows/build.yml`) runs on `macos-latest` because iOS/MacCatalyst TFMs require macOS. Building all TFMs locally on Windows skips the Apple targets. **Publishing** is manual (`workflow_dispatch` on `publish.yml`); it reads `<Version>` from the csproj, pushes to NuGet + GitHub Packages, and creates a GitHub release `v<version>`. Bump `<Version>` in `Maui.CredentialManager.csproj` to release.

### Tests

The build workflow runs `dotnet test` and collects coverage, but `tests/Maui.CredentialManagers.Tests` currently has no source files and is not referenced by `src/Maui.CredentialManager.slnx`, so no tests actually execute yet. If you add tests, wire the test project into the solution.

## Architecture

### Partial-class-per-platform pattern

`CredentialManagerService` is a single `partial class` whose implementation is split across files, with the MAUI build system compiling only the one matching the current TFM:

- `Services/CredentialManagerService.cs` — shared partial: holds `_options` field and declares the `partial` method signatures.
- `Services/ICredentialManagerService.cs` — the public contract consumers depend on. XML docs here are the source of truth for per-platform behavior.
- `Platforms/Android/Services/CredentialManagerService.cs` — Android impl + Android-only constructor.
- `Platforms/iOS/Services/CredentialManagerService.cs` — iOS impl + iOS-only constructor.
- `Platforms/MacCatalyst/Services/CredentialManagerService.cs` and `Platforms/Windows/Services/CredentialManagerService.cs` — stub impls + options-only constructor.

Because each platform file defines its own constructor, `IServiceCollectionExtensions.AddCredentialManagerService` uses `#if ANDROID / #elif IOS / #else` to construct the right one. **If you add or change a public method, you must update all five platform files plus the interface** or the build breaks on at least one TFM.

The platform `CredentialManagerService` orchestrates and shapes results; the actual native calls live in lower-level services it depends on:
- `Platforms/Android/Services/CredentialManagerAndroidService.cs` — wraps `AndroidX.Credentials.CredentialManager`.
- `Platforms/iOS/Services/CredentialManagerIosService.cs` — Keychain (`SecKeyChain`) + ASAuthorization bridge.

### Result pattern — methods don't throw

Every public method returns `CredentialManagerResultDto<T>` with `Data`, `ErrorMessage`, and `IsSuccess` (true when `ErrorMessage` is empty). Implementations wrap their bodies in try/catch and convert exceptions/missing-config into `ErrorMessage` rather than throwing. Preserve this — callers branch on `IsSuccess`, not exceptions.

### Android callback → Task bridge

`androidx.credentials` uses Java-style async callbacks. These are adapted to `Task` via `CallbackBase<TResult,TException>` (wraps a `TaskCompletionSource`, registers cancellation) and its concrete subclasses `CredentialManagerCallback` / `CredentialManagerVoidCallback`. **Note:** these concrete callbacks live in the legacy namespace `SatisFIT.Client.App.Platforms.Android.Services.Test` (carried over from the original project) — keep that in mind when searching; don't "fix" it casually.

### iOS delegate → Task bridge

ASAuthorization is delegate-based; `Platforms/iOS/Delegates/AuthorizationDelegate.cs` bridges the controller delegate callbacks into an awaitable `Task<ASAuthorization>`. Note that on iOS, saving a password uses Keychain `SecKeyChain.Add` while reading uses `ASAuthorizationPasswordProvider` — different APIs over the same Keychain store; this is the standard iOS pattern, not a bug.

### SSO model

`ContinueWithSso(SsoProvider, ...)` resolves `SsoProvider.PlatformDefault` to Google on Android and Apple on iOS. Each provider has a configurable `SsoAuthMethod` (`Native` or `Browser`):
- Android: Google is `Native` by default (Credential Manager); Apple is always `Browser` (no native Apple SDK on Android) — `AppleAuthMethod` has no setter.
- iOS: Apple is `Native` by default (ASAuthorization); Google is always `Browser` (no native Google SDK here) — `GoogleAuthMethod` has no setter.

Browser flows go through MAUI `WebAuthenticator` against Google/Apple OAuth endpoints and validate the `state` parameter to guard against CSRF. Config lives in `Models/Options/CredentialManagerOptions.cs` (shared `GoogleServerClientId`/`AppleServiceId`/`AppleRedirectUri` plus nested `Android` / `Ios` option objects for redirect URIs, callback schemes, and per-platform auth methods).

### DI lifetime

Services are registered `AddScoped` (not singleton). The interface `ICredentialManagerService` resolves to a `CredentialManagerService` built with the platform's low-level service + the configured `CredentialManagerOptions` singleton.

### Global usings

`Usings.cs` defines Android-only global aliases (`CredentialManager`, `CreateCredentialResponse` → the `AndroidX.Credentials.*` types) to disambiguate from the library's own `CredentialManager` namespace. `ImplicitUsings` and `Nullable` are both enabled.

## Conventions

- Code style is enforced via `.editorconfig`. The codebase uses file-scoped namespaces, nullable reference types, and `string.IsNullOrEmpty` for guards.
- Per-method/per-type XML docs are required (`GenerateDocumentationFile` is on) — keep the interface docs accurate since they document platform divergence.
