#pragma warning disable CS1998 // Async method lacks 'await' operators
using Maui.CredentialManagers.Models;
using Maui.CredentialManagers.Models.Options;

namespace Maui.CredentialManagers.Services;

public partial class CredentialManagerService
{
    public CredentialManagerService(CredentialManagerOptions options)
    {
        _options = options;
    }

    public async partial Task<CredentialManagerResultDto<bool>> CreatePasswordCredential(
        PasswordCredentialDto passwordCredential, CancellationToken cancellationToken)
    {
        return new CredentialManagerResultDto<bool>
        {
            ErrorMessage = "Credential management is not supported on macOS"
        };
    }

    public async partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(
        CancellationToken cancellationToken)
    {
        return new CredentialManagerResultDto<CredentialDto>
        {
            ErrorMessage = "Credential management is not supported on macOS"
        };
    }

    public async partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(
        GetPasswordCredentialOptionsDto getPasswordCredentialOptionsDto, CancellationToken cancellationToken)
    {
        return new CredentialManagerResultDto<CredentialDto>
        {
            ErrorMessage = "Credential management is not supported on macOS"
        };
    }

    public async partial Task<CredentialManagerResultDto<CredentialDto>> ContinueWithSso(
        SsoProvider provider, CancellationToken cancellationToken)
    {
        return new CredentialManagerResultDto<CredentialDto>
        {
            ErrorMessage = "SSO is not supported on macOS"
        };
    }

    public async partial Task<CredentialManagerResultDto<bool>> ClearCredentialState(
        CancellationToken cancellationToken)
    {
        return new CredentialManagerResultDto<bool>
        {
            ErrorMessage = "Credential management is not supported on macOS"
        };
    }
}
#pragma warning restore CS1998
