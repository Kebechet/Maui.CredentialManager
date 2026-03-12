using Android.Runtime;
using AndroidX.Credentials;
using Maui.CredentialManager.Platforms.Android.Callbacks.Base;

namespace Maui.CredentialManager.Platforms.Android.Callbacks;

internal class CredentialManagerVoidCallback<TException> : VoidCallbackBase<TException>, ICredentialManagerCallback
    where TException : Java.Lang.Exception
{
    public CredentialManagerVoidCallback(CancellationToken cancellationToken) : base(cancellationToken)
    {
    }

    public void OnResult(Java.Lang.Object? result)
    {
        ReportSuccess();
    }

    public void OnError(Java.Lang.Object e)
    {
        var exception = e.JavaCast<TException>();
        ReportException(exception);
    }
}