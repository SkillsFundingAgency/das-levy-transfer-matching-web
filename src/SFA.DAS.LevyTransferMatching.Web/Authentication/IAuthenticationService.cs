namespace SFA.DAS.LevyTransferMatching.Web.Authentication
{
    public interface IAuthenticationService
    {
        bool IsUserAuthenticated();
        bool TryGetUserClaimValue(string key, out string value);
        string UserDisplayName { get; }
        string UserId { get; }
        string UserEmail { get; }
    }
}
