namespace Maui.CredentialManager.Models;

public class CredentialManagerResultDto<TData>
{
    public string? ErrorMessage { get; set; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    public TData? Data { get; set; }
}
