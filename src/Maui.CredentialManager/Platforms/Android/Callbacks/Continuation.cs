//using Java.Lang;
//using Kotlin.Coroutines;

//namespace Maui.CredentialManager.Platforms.Android.Callbacks;

//internal class Continuation : Java.Lang.Object, IContinuation
//{
//    public ICoroutineContext Context => EmptyCoroutineContext.Instance;

//    //private TaskCompletionSource<Java.Lang.Object> _tcs;

//    private readonly TaskCompletionSource _taskCompletionSource;

//    public Continuation(TaskCompletionSource taskCompletionSource, CancellationToken cancellationToken)
//    {
//        _taskCompletionSource = taskCompletionSource;
//        cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
//    }

//    public void ResumeWith(Java.Lang.Object result)
//    {
//        var exception = result as Throwable;
//        if (exception != null)
//        {
//            _taskCompletionSource.TrySetException(new Exception(exception.Message));
//        }
//        else
//        {
//            _taskCompletionSource.TrySetResult(result as Java.Lang.Object);
//        }
//    }
//}


//public async Task ClearCredentialState()
//{
//    var request = new ClearCredentialStateRequest();
//    var tcs = new TaskCompletionSource();

//    _credentialManager.ClearCredentialState(
//        request,
//        new Continuation(tcs, default)
//    );

//    await tcs.Task;
//}