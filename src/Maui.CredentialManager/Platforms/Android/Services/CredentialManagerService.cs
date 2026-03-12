using AndroidX.Credentials;
using Maui.CredentialManager.Models;
using Maui.CredentialManager.Models.Options;
using Maui.CredentialManager.Platforms.Android.Services;
using Xamarin.GoogleAndroid.Libraries.Identity.GoogleId;

namespace Maui.CredentialManager.Services;

//https://developer.android.com/training/sign-in/passkeys
//https://developer.android.com/reference/androidx/credentials/CredentialManager
public partial class CredentialManagerService
{
    private readonly CredentialManagerAndroidService _credentialManagerAndroidService;

    public CredentialManagerService(CredentialManagerAndroidService credentialManagerAndroidService, CredentialManagerOptions options)
    {
        _credentialManagerAndroidService = credentialManagerAndroidService;
        _options = options;
    }

    public async partial Task<CredentialManagerResultDto<bool>> CreatePasswordCredential(PasswordCredentialDto passwordCredential, CancellationToken cancellationToken)
    {
        var createPasswordRequest = new CreatePasswordRequest(passwordCredential.Id, passwordCredential.Password);

        try
        {
            var res = await _credentialManagerAndroidService.CreatePassword(createPasswordRequest, cancellationToken);
            if (res is null)
            {
                return new CredentialManagerResultDto<bool>
                {
                    ErrorMessage = "No password credential found"
                };
            }

            return new CredentialManagerResultDto<bool>
            {
                Data = true
            };
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<bool>
            {
                ErrorMessage = e.Message
            };
        }
    }

    public async partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(CancellationToken cancellationToken)
    {
        return await GetPasswordCredential(new GetPasswordCredentialOptionsDto(), cancellationToken);
    }

    public async partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(GetPasswordCredentialOptionsDto getPasswordCredentialOptionsDto, CancellationToken cancellationToken)
    {
        var passwordOption = new GetPasswordOption();

        var requestBuilder = new GetCredentialRequest.Builder()
            .AddCredentialOption(passwordOption);

        if (!string.IsNullOrEmpty(_options.GoogleServerClientId))
        {
            var googleIdOption = new GetGoogleIdOption.Builder()
               .SetFilterByAuthorizedAccounts(getPasswordCredentialOptionsDto.OnlyAuthorizedAccounts)
               .SetServerClientId(_options.GoogleServerClientId)
               .SetNonce(Guid.NewGuid().ToString())
               .SetAutoSelectEnabled(getPasswordCredentialOptionsDto.IsCredentialAutoSelectEnabled)
               .SetRequestVerifiedPhoneNumber(getPasswordCredentialOptionsDto.RequestVerifiedPhoneNumber)
               .Build();

            requestBuilder.AddCredentialOption(googleIdOption);
        }

        var getCredentialRequest = requestBuilder
            .SetPreferIdentityDocUi(getPasswordCredentialOptionsDto.PreferIdentityDocUi)
            .SetPreferImmediatelyAvailableCredentials(getPasswordCredentialOptionsDto.PreferImmediatelyAvailableCredentials)
            .Build();

        return await ProcessGetCredentialRequest(getCredentialRequest, cancellationToken);
    }

    //https://developers.google.com/identity/android-credential-manager/android/reference/com/google/android/libraries/identity/googleid/GetSignInWithGoogleOption
    public async partial Task<CredentialManagerResultDto<CredentialDto>> ContinueWithSso(SsoProvider provider, CancellationToken cancellationToken)
    {
        var resolvedProvider = provider == SsoProvider.PlatformDefault
            ? SsoProvider.Google
            : provider;

        return resolvedProvider switch
        {
            SsoProvider.Google => _options.Android.GoogleAuthMethod switch
            {
                SsoAuthMethod.Browser => await HandleGoogleSignInBrowser(cancellationToken),
                _ => await HandleGoogleSignIn(cancellationToken)
            },
            SsoProvider.Apple => await HandleAppleSignIn(cancellationToken),
            _ => new CredentialManagerResultDto<CredentialDto>
            {
                ErrorMessage = $"Unsupported SSO provider: {resolvedProvider}"
            }
        };
    }

    public async partial Task<CredentialManagerResultDto<bool>> ClearCredentialState(CancellationToken cancellationToken)
    {
        try
        {
            await _credentialManagerAndroidService.ClearCredentialState(cancellationToken);
            return new CredentialManagerResultDto<bool> { Data = true };
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<bool> { ErrorMessage = e.Message };
        }
    }

    private async Task<CredentialManagerResultDto<CredentialDto>> HandleGoogleSignIn(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_options.GoogleServerClientId))
        {
            return new CredentialManagerResultDto<CredentialDto>
            {
                ErrorMessage = "Google Sign-In requires GoogleServerClientId to be configured"
            };
        }

        var signInWithGoogleOption = new GetSignInWithGoogleOption(_options.GoogleServerClientId, "", Guid.NewGuid().ToString());

        var getCredentialRequest = new GetCredentialRequest.Builder()
            .AddCredentialOption(signInWithGoogleOption)
            .Build();

        return await ProcessGetCredentialRequest(getCredentialRequest, cancellationToken);
    }

    private async Task<CredentialManagerResultDto<CredentialDto>> HandleGoogleSignInBrowser(CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.GoogleServerClientId) ||
                string.IsNullOrEmpty(_options.Android.GoogleRedirectUri) ||
                string.IsNullOrEmpty(_options.Android.GoogleCallbackScheme))
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "Google Sign-In via browser on Android requires GoogleServerClientId, Android.GoogleRedirectUri, and Android.GoogleCallbackScheme to be configured"
                };
            }

            var nonce = Guid.NewGuid().ToString();
            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                          $"?client_id={Uri.EscapeDataString(_options.GoogleServerClientId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(_options.Android.GoogleRedirectUri)}" +
                          $"&response_type=id_token&scope=openid%20email%20profile&nonce={nonce}";

            var result = await WebAuthenticator.Default.AuthenticateAsync(
                new Uri(authUrl), new Uri(_options.Android.GoogleCallbackScheme));

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

    private async Task<CredentialManagerResultDto<CredentialDto>> HandleAppleSignIn(CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.AppleServiceId) ||
                string.IsNullOrEmpty(_options.AppleRedirectUri) ||
                string.IsNullOrEmpty(_options.Android.AppleCallbackScheme))
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "Apple Sign-In on Android requires AppleServiceId, AppleRedirectUri, and Android.AppleCallbackScheme to be configured"
                };
            }

            var state = Guid.NewGuid().ToString();
            var nonce = Guid.NewGuid().ToString();
            var authUrl = $"https://appleid.apple.com/auth/authorize" +
                          $"?client_id={Uri.EscapeDataString(_options.AppleServiceId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(_options.AppleRedirectUri)}" +
                          $"&response_type=code%20id_token&scope=name%20email" +
                          $"&response_mode=fragment&state={state}&nonce={nonce}";

            var result = await WebAuthenticator.Default.AuthenticateAsync(
                new Uri(authUrl), new Uri(_options.Android.AppleCallbackScheme));

            var returnedState = result.Properties.GetValueOrDefault("state");
            if (returnedState != state)
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "Apple Sign-In state mismatch — possible CSRF attack"
                };
            }

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

    private async Task<CredentialManagerResultDto<CredentialDto>> ProcessGetCredentialRequest(GetCredentialRequest getCredentialRequest, CancellationToken cancellationToken)
    {
        try
        {
            var res = await _credentialManagerAndroidService.GetCredential(getCredentialRequest, cancellationToken);
            if (res is null || res.Credential is null)
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "No password credential found"
                };
            }

            if (res.Credential.GetType() == typeof(PublicKeyCredential))
            {
                var responseJson = ((PublicKeyCredential)res.Credential).AuthenticationResponseJson;

                return new CredentialManagerResultDto<CredentialDto>
                {
                    Data = new CredentialDto
                    {
                        PublicKeyCredential = new PublicKeyCredentialDto
                        {
                            AuthenticationResponseJson = responseJson
                        }
                    }
                };
            }
            else if (res.Credential.GetType() == typeof(PasswordCredential))
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    Data = new CredentialDto
                    {
                        PasswordCredential = new PasswordCredentialDto
                        {
                            Id = ((PasswordCredential)res.Credential).Id,
                            Password = ((PasswordCredential)res.Credential).Password
                        }
                    }
                };
            }
            else if (res.Credential.GetType() == typeof(CustomCredential))
            {
                try
                {
                    var googleIdTokenCredential = GoogleIdTokenCredential.CreateFrom(((CustomCredential)res.Credential).Data);

                    return new CredentialManagerResultDto<CredentialDto>
                    {
                        Data = new CredentialDto
                        {
                            GoogleIdTokenCredential = new GoogleIdTokenCredentialDto
                            {
                                Id = googleIdTokenCredential.Id,
                                IdToken = googleIdTokenCredential.IdToken,
                                DisplayName = googleIdTokenCredential.DisplayName,
                                FamilyName = googleIdTokenCredential.FamilyName,
                                GivenName = googleIdTokenCredential.GivenName,
                                ProfilePictureUri = googleIdTokenCredential.ProfilePictureUri?.ToString(),
                                PhoneNumber = googleIdTokenCredential.PhoneNumber
                            }
                        }
                    };
                }
                catch (GoogleIdTokenParsingException)
                {
                    return new CredentialManagerResultDto<CredentialDto>
                    {
                        ErrorMessage = "Received an invalid Google ID token response"
                    };
                }
            }
            else
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "Unexpected type of credential"
                };
            }
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<CredentialDto>
            {
                ErrorMessage = e.Message
            };
        }
    }
}
