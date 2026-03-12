namespace Maui.CredentialManager.Platforms.Android.Callbacks.Base;

internal abstract class CallbackBase<TResult, TException> : Java.Lang.Object
    where TResult : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    private readonly TaskCompletionSource<TResult?> _taskCompletionSource;

    public CallbackBase(CancellationToken cancellationToken)
    {
        _taskCompletionSource = new TaskCompletionSource<TResult?>();
        cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
    }

    public Task<TResult?> Task => _taskCompletionSource.Task;

    protected void ReportSuccess(TResult? result)
    {
        _taskCompletionSource.TrySetResult(result);
    }

    protected void ReportException(TException exception)
    {
        _taskCompletionSource.TrySetException(exception);
    }
}