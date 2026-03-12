using Maui.CredentialManager.Models;
using Maui.CredentialManager.Models.Options;

namespace Maui.CredentialManager.Services;

/// <summary>
/// Provides cross-platform credential management operations for Android and iOS.
/// </summary>
public interface ICredentialManagerService
{
    /// <summary>
    /// Stores a password credential in the platform's credential manager.
    /// On Android uses Credential Manager API, on iOS uses Keychain Services.
    /// Note: On iOS, <see cref="GetPasswordCredential(CancellationToken)"/> uses ASAuthorizationPasswordProvider
    /// which reads from the same Keychain store — the different APIs are the standard iOS pattern.
    /// </summary>
    /// <param name="passwordCredential">The username and password to store.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or containing an error message.</returns>
    Task<CredentialManagerResultDto<bool>> CreatePasswordCredential(
        PasswordCredentialDto passwordCredential, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a stored password credential using default options.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result containing the credential or an error message.</returns>
    Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a stored password credential with the specified options.
    /// </summary>
    /// <param name="options">Options controlling credential retrieval behavior such as auto-select and filtering.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result containing the credential or an error message.</returns>
    Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(
        GetPasswordCredentialOptionsDto options, CancellationToken cancellationToken);

    /// <summary>
    /// Initiates a Single Sign-On flow with the specified provider.
    /// <see cref="SsoProvider.PlatformDefault"/> resolves to Google on Android and Apple on iOS.
    /// Auth method per provider: Google on Android is configurable (native/browser via <see cref="CredentialManagerOptions.Android"/>),
    /// Apple on iOS is configurable (native/browser via <see cref="CredentialManagerOptions.Ios"/>).
    /// Google on iOS and Apple on Android are always browser-based (no native SDK available).
    /// </summary>
    /// <param name="provider">The SSO provider to authenticate with.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result containing the SSO credential (Google ID token or Apple ID) or an error message.</returns>
    Task<CredentialManagerResultDto<CredentialDto>> ContinueWithSso(
        SsoProvider provider, CancellationToken cancellationToken);

    /// <summary>
    /// Clears all stored credential state from the platform's credential manager.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or containing an error message.</returns>
    Task<CredentialManagerResultDto<bool>> ClearCredentialState(
        CancellationToken cancellationToken);
}
