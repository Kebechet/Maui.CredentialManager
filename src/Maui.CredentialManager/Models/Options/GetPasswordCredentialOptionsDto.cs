namespace Maui.CredentialManager.Models.Options;

public class GetPasswordCredentialOptionsDto
{
    public bool OnlyAuthorizedAccounts { get; set; } = false;
    public bool IsCredentialAutoSelectEnabled { get; set; } = false;
    public bool RequestVerifiedPhoneNumber { get; set; } = false;

    public bool PreferIdentityDocUi { get; set; } = false;
    public bool PreferImmediatelyAvailableCredentials { get; set; } = false;
}
