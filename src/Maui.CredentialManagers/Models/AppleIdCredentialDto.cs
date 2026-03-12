namespace Maui.CredentialManagers.Models;

public class AppleIdCredentialDto
{
    public required string UserId { get; set; }
    public required string IdToken { get; set; }
    public string? Email { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? RealUserStatus { get; set; }
}
