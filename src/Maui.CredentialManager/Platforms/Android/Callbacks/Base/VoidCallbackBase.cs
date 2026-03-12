namespace Maui.CredentialManager.Platforms.Android.Callbacks.Base;

internal abstract class VoidCallbackBase<TException> : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    private readonly TaskCompletionSource _taskCompletionSource;

    public VoidCallbackBase(CancellationToken cancellationToken)
    {
        _taskCompletionSource = new TaskCompletionSource();
        cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
    }

    public Task Task => _taskCompletionSource.Task;

    protected void ReportSuccess()
    {
        _taskCompletionSource.TrySetResult();
    }

    protected void ReportException(TException exception)
    {
        _taskCompletionSource.TrySetException(exception);
    }
}