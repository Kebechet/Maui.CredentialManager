using AuthenticationServices;
using Foundation;
using Maui.CredentialManagers.Models;
using Maui.CredentialManagers.Platforms.iOS.Delegates;
using Security;

namespace Maui.CredentialManagers.Platforms.iOS.Services;

public class CredentialManagerIosService
{
    public SecStatusCode StorePassword(string server, string account, string password)
    {
        // Remove existing entry first to allow update
        var removeQuery = new SecRecord(SecKind.InternetPassword)
        {
            Server = server,
            Account = account
        };
        SecKeyChain.Remove(removeQuery);

        var record = new SecRecord(SecKind.InternetPassword)
        {
            Server = server,
            Account = account,
            ValueData = NSData.FromString(password, NSStringEncoding.UTF8),
            Accessible = SecAccessible.WhenUnlockedThisDeviceOnly
        };

        return SecKeyChain.Add(record);
    }

    public PasswordCredentialDto? RetrievePassword(string server)
    {
        var query = new SecRecord(SecKind.InternetPassword)
        {
            Server = server
        };

        var record = SecKeyChain.QueryAsRecord(query, out var status);
        if (status != SecStatusCode.Success || record is null)
            return null;

        var password = record.ValueData is not null
            ? NSString.FromData(record.ValueData, NSStringEncoding.UTF8)?.ToString()
            : null;

        if (record.Account is null || password is null)
            return null;

        return new PasswordCredentialDto
        {
            Id = record.Account,
            Password = password
        };
    }

    public SecStatusCode RemovePasswords(string? server = null)
    {
        var query = new SecRecord(SecKind.InternetPassword);
        if (server is not null)
            query.Server = server;

        return SecKeyChain.Remove(query);
    }

    public async Task<ASAuthorization> PerformAppleSignIn()
    {
        var provider = new ASAuthorizationAppleIdProvider();
        var request = provider.CreateRequest();
        request.RequestedScopes = [ASAuthorizationScope.Email, ASAuthorizationScope.FullName];

        var authDelegate = new AuthorizationDelegate();
        var controller = new ASAuthorizationController([request])
        {
            Delegate = authDelegate,
            PresentationContextProvider = authDelegate
        };

        controller.PerformRequests();
        return await authDelegate.Task;
    }

    public async Task<ASAuthorization> PerformPasswordRequest()
    {
        var passwordProvider = new ASAuthorizationPasswordProvider();
        var passwordRequest = passwordProvider.CreateRequest();

        var authDelegate = new AuthorizationDelegate();
        var controller = new ASAuthorizationController([passwordRequest])
        {
            Delegate = authDelegate,
            PresentationContextProvider = authDelegate
        };

        controller.PerformRequests();
        return await authDelegate.Task;
    }
}
