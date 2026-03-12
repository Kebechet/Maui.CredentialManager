using Maui.CredentialManagers.Models;
using Maui.CredentialManagers.Models.Options;

namespace Maui.CredentialManagers.Services;

public partial class CredentialManagerService : ICredentialManagerService
{
    private readonly CredentialManagerOptions _options;

    public partial Task<CredentialManagerResultDto<bool>> CreatePasswordCredential(
        PasswordCredentialDto passwordCredential, CancellationToken cancellationToken);

    public partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(
        CancellationToken cancellationToken);

    public partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(
        GetPasswordCredentialOptionsDto getPasswordCredentialOptionsDto, CancellationToken cancellationToken);

    public partial Task<CredentialManagerResultDto<CredentialDto>> ContinueWithSso(
        SsoProvider provider, CancellationToken cancellationToken);

    public partial Task<CredentialManagerResultDto<bool>> ClearCredentialState(
        CancellationToken cancellationToken);
}
