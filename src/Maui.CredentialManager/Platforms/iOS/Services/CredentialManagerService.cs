using AuthenticationServices;
using Maui.CredentialManager.Models;
using Maui.CredentialManager.Models.Options;
using Maui.CredentialManager.Platforms.iOS.Services;

namespace Maui.CredentialManager.Services;

public partial class CredentialManagerService
{
    private readonly CredentialManagerIosService _credentialManagerIosService;

    public CredentialManagerService(CredentialManagerIosService credentialManagerIosService, CredentialManagerOptions options)
    {
        _credentialManagerIosService = credentialManagerIosService;
        _options = options;
    }

    public async partial Task<CredentialManagerResultDto<bool>> CreatePasswordCredential(
        PasswordCredentialDto passwordCredential, CancellationToken cancellationToken)
    {
        try
        {
            var status = _credentialManagerIosService.StorePassword(
                _options.Ios.GoogleRedirectUri ?? "default",
                passwordCredential.Id,
                passwordCredential.Password);

            if (status != Security.SecStatusCode.Success)
            {
                return new CredentialManagerResultDto<bool>
                {
                    ErrorMessage = $"Failed to store password in Keychain: {status}"
                };
            }

            return new CredentialManagerResultDto<bool> { Data = true };
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<bool> { ErrorMessage = e.Message };
        }
    }

    public async partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(
        CancellationToken cancellationToken)
    {
        return await GetPasswordCredential(new GetPasswordCredentialOptionsDto(), cancellationToken);
    }

    public async partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(
        GetPasswordCredentialOptionsDto getPasswordCredentialOptionsDto, CancellationToken cancellationToken)
    {
        try
        {
            var authorization = await _credentialManagerIosService.PerformPasswordRequest();

            if (authorization.GetCredential<ASPasswordCredential>() is { } passwordCredential)
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    Data = new CredentialDto
                    {
                        PasswordCredential = new PasswordCredentialDto
                        {
                            Id = passwordCredential.User,
                            Password = passwordCredential.Password
                        }
                    }
                };
            }

            return new CredentialManagerResultDto<CredentialDto>
            {
                ErrorMessage = "No password credential found"
            };
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<CredentialDto> { ErrorMessage = e.Message };
        }
    }

    public async partial Task<CredentialManagerResultDto<CredentialDto>> ContinueWithSso(
        SsoProvider provider, CancellationToken cancellationToken)
    {
        var resolvedProvider = provider == SsoProvider.PlatformDefault
            ? SsoProvider.Apple
            : provider;

        return resolvedProvider switch
        {
            SsoProvider.Apple => _options.Ios.AppleAuthMethod switch
            {
                SsoAuthMethod.Browser => await HandleAppleSignInBrowser(cancellationToken),
                _ => await HandleAppleSignIn(cancellationToken)
            },
            SsoProvider.Google => await HandleGoogleSignIn(cancellationToken),
            _ => new CredentialManagerResultDto<CredentialDto>
            {
                ErrorMessage = $"Unsupported SSO provider: {resolvedProvider}"
            }
        };
    }

    public async partial Task<CredentialManagerResultDto<bool>> ClearCredentialState(
        CancellationToken cancellationToken)
    {
        try
        {
            _credentialManagerIosService.RemovePasswords();
            return new CredentialManagerResultDto<bool> { Data = true };
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<bool> { ErrorMessage = e.Message };
        }
    }

    private async Task<CredentialManagerResultDto<CredentialDto>> HandleAppleSignIn(
        CancellationToken cancellationToken)
    {
        try
        {
            var authorization = await _credentialManagerIosService.PerformAppleSignIn();

            if (authorization.GetCredential<ASAuthorizationAppleIdCredential>() is { } appleCredential)
            {
                var idToken = appleCredential.IdentityToken is not null
                    ? Foundation.NSString.FromData(appleCredential.IdentityToken, Foundation.NSStringEncoding.UTF8)?.ToString()
                    : null;

                return new CredentialManagerResultDto<CredentialDto>
                {
                    Data = new CredentialDto
                    {
                        AppleIdCredential = new AppleIdCredentialDto
                        {
                            UserId = appleCredential.User,
                            IdToken = idToken ?? "",
                            Email = appleCredential.Email,
                            GivenName = appleCredential.FullName?.GivenName,
                            FamilyName = appleCredential.FullName?.FamilyName,
                            RealUserStatus = appleCredential.RealUserStatus.ToString()
                        }
                    }
                };
            }

            return new CredentialManagerResultDto<CredentialDto>
            {
                ErrorMessage = "Apple Sign-In did not return a valid credential"
            };
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<CredentialDto> { ErrorMessage = e.Message };
        }
    }

    private async Task<CredentialManagerResultDto<CredentialDto>> HandleAppleSignInBrowser(
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.AppleServiceId) ||
                string.IsNullOrEmpty(_options.AppleRedirectUri) ||
                string.IsNullOrEmpty(_options.Ios.AppleCallbackScheme))
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "Apple Sign-In via browser on iOS requires AppleServiceId, AppleRedirectUri, and Ios.AppleCallbackScheme to be configured"
                };
            }

            var state = Guid.NewGuid().ToString();
            var nonce = Guid.NewGuid().ToString();
            var authUrl = $"https://appleid.apple.com/auth/authorize" +
                          $"?client_id={Uri.EscapeDataString(_options.AppleServiceId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(_options.AppleRedirectUri)}" +
                          $"&response_type=code%20id_token&scope=name%20email" +
                          $"&response_mode=form_post&state={state}&nonce={nonce}";

            var result = await WebAuthenticator.Default.AuthenticateAsync(
                new Uri(authUrl), new Uri(_options.Ios.AppleCallbackScheme));

            var idToken = result.IdToken ?? result.Properties.GetValueOrDefault("id_token");
            if (string.IsNullOrEmpty(idToken))
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "Apple Sign-In did not return a valid token"
                };
            }

            return new CredentialManagerResultDto<CredentialDto>
            {
                Data = new CredentialDto
                {
                    AppleIdCredential = new AppleIdCredentialDto
                    {
                        UserId = result.Properties.GetValueOrDefault("user") ?? "",
                        IdToken = idToken,
                        Email = result.Properties.GetValueOrDefault("email")
                    }
                }
            };
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<CredentialDto> { ErrorMessage = e.Message };
        }
    }

    private async Task<CredentialManagerResultDto<CredentialDto>> HandleGoogleSignIn(
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.Ios.GoogleClientId) || string.IsNullOrEmpty(_options.Ios.GoogleRedirectUri))
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "Google Sign-In on iOS requires Ios.GoogleClientId and Ios.GoogleRedirectUri to be configured"
                };
            }

            var nonce = Guid.NewGuid().ToString();
            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                          $"?client_id={Uri.EscapeDataString(_options.Ios.GoogleClientId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(_options.Ios.GoogleRedirectUri)}" +
                          $"&response_type=id_token&scope=openid%20email%20profile&nonce={nonce}";

            var result = await WebAuthenticator.Default.AuthenticateAsync(
                new Uri(authUrl), new Uri(_options.Ios.GoogleRedirectUri));

            var idToken = result.IdToken ?? result.AccessToken;
            if (string.IsNullOrEmpty(idToken))
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "Google Sign-In did not return a valid token"
                };
            }

            return new CredentialManagerResultDto<CredentialDto>
            {
                Data = new CredentialDto
                {
                    GoogleIdTokenCredential = new GoogleIdTokenCredentialDto
                    {
                        Id = result.Properties.GetValueOrDefault("email") ?? "",
                        IdToken = idToken
                    }
                }
            };
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<CredentialDto> { ErrorMessage = e.Message };
        }
    }
}
