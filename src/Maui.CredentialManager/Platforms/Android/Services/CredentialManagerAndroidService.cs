using Android.Annotation;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.Content;
using AndroidX.Credentials;
using AndroidX.Credentials.Exceptions;
using Maui.CredentialManager.Platforms.Android.Callbacks;
using SatisFIT.Client.App.Platforms.Android.Services.Test;

namespace Maui.CredentialManager.Platforms.Android.Services;

public class CredentialManagerAndroidService
{
    private readonly ICredentialManager _credentialManager;
    private Context _activityContext => Platform.CurrentActivity ??
        throw new Exception("Current activity is null");

    public CredentialManagerAndroidService()
    {
        _credentialManager = AndroidX.Credentials.CredentialManager.Create(_activityContext);
    }

    public async Task<CreatePublicKeyCredentialResponse?> CreatePublicKeyCredential(CreatePublicKeyCredentialRequest request, CancellationToken cancellationToken)
    {
        var response = await CreateCredential(request, cancellationToken);
        return (CreatePublicKeyCredentialResponse?)response;
    }
    public async Task<CreateCustomCredentialResponse?> CreateCustomCredential(CreateCustomCredentialRequest request, CancellationToken cancellationToken)
    {
        var response = await CreateCredential(request, cancellationToken);
        return (CreateCustomCredentialResponse?)response;
    }
    public async Task<CreatePasswordResponse?> CreatePassword(CreatePasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await CreateCredential(request, cancellationToken);
        return (CreatePasswordResponse?)response;
    }
    [TargetApi(Value = 34)]
    public PendingIntent CreateSettingsPendingIntent()
    {
        return _credentialManager.CreateSettingsPendingIntent();
    }

    public async Task<GetCredentialResponse?> GetCredential(GetCredentialRequest request, CancellationToken cancellationToken)
    {
        var callback = new CredentialManagerCallback<GetCredentialResponse, GetCredentialException>(cancellationToken);

        _credentialManager.GetCredentialAsync(
            _activityContext,
            request,
            null,
            ContextCompat.GetMainExecutor(_activityContext),
            callback
        );

        return await callback.Task;
    }

    [TargetApi(Value = 34)]
    public async Task<PrepareGetCredentialResponse?> PrepareGetCredential(GetCredentialRequest request, CancellationToken cancellationToken)
    {
        var callback = new CredentialManagerCallback<PrepareGetCredentialResponse, GetCredentialException>(cancellationToken);

        _credentialManager.PrepareGetCredentialAsync(
            request,
            null,
            ContextCompat.GetMainExecutor(_activityContext),
            callback
        );

        return await callback.Task;
    }

    public async Task ClearCredentialState(CancellationToken cancellationToken)
    {
        var request = new ClearCredentialStateRequest();
        var callback = new CredentialManagerVoidCallback<ClearCredentialException>(cancellationToken);

        var cancellationSignal = new CancellationSignal();
        cancellationToken.Register(() => cancellationSignal.Cancel());

        _credentialManager.ClearCredentialStateAsync(
            request,
            cancellationSignal,
            ContextCompat.GetMainExecutor(_activityContext),
            callback
        );

        await callback.Task;
    }

    private Task<CreateCredentialResponse?> CreateCredential(CreateCredentialRequest request, CancellationToken cancellationToken)
    {
        var callback = new CredentialManagerCallback<CreateCredentialResponse, CreateCredentialException>(cancellationToken);

        _credentialManager.CreateCredentialAsync(
            _activityContext,
            request,
            null,
            ContextCompat.GetMainExecutor(_activityContext),
            callback
        );

        return callback.Task;
    }
}