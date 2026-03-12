using AuthenticationServices;
using Foundation;
using UIKit;

namespace Maui.CredentialManager.Platforms.iOS.Delegates;

internal sealed class AuthorizationDelegate : NSObject, IASAuthorizationControllerDelegate,
    IASAuthorizationControllerPresentationContextProviding
{
    private readonly TaskCompletionSource<ASAuthorization> _tcs = new();

    public Task<ASAuthorization> Task => _tcs.Task;

    [Export("authorizationController:didCompleteWithAuthorization:")]
    public void DidComplete(ASAuthorizationController controller, ASAuthorization authorization)
        => _tcs.TrySetResult(authorization);

    [Export("authorizationController:didCompleteWithError:")]
    public void DidComplete(ASAuthorizationController controller, NSError error)
        => _tcs.TrySetException(new NSErrorException(error));

    public UIWindow GetPresentationAnchor(ASAuthorizationController controller)
    {
        var scene = UIApplication.SharedApplication.ConnectedScenes
            .ToArray<UIScene>()
            .OfType<UIWindowScene>()
            .FirstOrDefault();

        return scene?.KeyWindow
            ?? scene?.Windows.FirstOrDefault()
#pragma warning disable CA1422 // Last-resort fallback for single-scene apps
            ?? UIApplication.SharedApplication.KeyWindow
#pragma warning restore CA1422
            ?? throw new InvalidOperationException("No presentation anchor window found");
    }
}
