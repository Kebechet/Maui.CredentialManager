namespace Maui.CredentialManagers.Models;

public class CredentialDto
{
    public PublicKeyCredentialDto? PublicKeyCredential { get; set; }
    public PasswordCredentialDto? PasswordCredential { get; set; }
    public GoogleIdTokenCredentialDto? GoogleIdTokenCredential { get; set; }
    public AppleIdCredentialDto? AppleIdCredential { get; set; }

    public bool IsPublicKeyCredential => PublicKeyCredential is not null;
    public bool IsPasswordCredential => PasswordCredential is not null;
    public bool IsGoogleIdTokenCredential => GoogleIdTokenCredential is not null;
    public bool IsAppleIdCredential => AppleIdCredential is not null;

    public bool IsEmpty => PublicKeyCredential is null && PasswordCredential is null
        && GoogleIdTokenCredential is null && AppleIdCredential is null;
}
