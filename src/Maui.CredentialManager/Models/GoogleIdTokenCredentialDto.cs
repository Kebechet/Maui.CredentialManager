namespace Maui.CredentialManager.Models;

//https://developers.google.com/identity/android-credential-manager/android/reference/com/google/android/libraries/identity/googleid/GoogleIdTokenCredential
public class GoogleIdTokenCredentialDto
{
    public required string Id { get; set; }
    public required string IdToken { get; set; }
    public string? DisplayName { get; set; }
    public string? FamilyName { get; set; }
    public string? GivenName { get; set; }
    public string? ProfilePictureUri { get; set; }
    public string? PhoneNumber { get; set; }
}
