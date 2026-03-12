using Android.Runtime;
using AndroidX.Credentials;
using Maui.CredentialManager.Platforms.Android.Callbacks.Base;

namespace SatisFIT.Client.App.Platforms.Android.Services.Test;

internal sealed class CredentialManagerCallback<TResult, TException> : CallbackBase<TResult, TException>, ICredentialManagerCallback
    where TResult : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    public CredentialManagerCallback(CancellationToken cancellationToken) : base(cancellationToken)
    {
    }

    public void OnResult(Java.Lang.Object? result)
    {
        var parsedResult = result is not null
            ? (TResult)result
            : null;

        ReportSuccess(parsedResult);
    }

    public void OnError(Java.Lang.Object e)
    {
        var exception = e.JavaCast<TException>();
        ReportException(exception);
    }
}