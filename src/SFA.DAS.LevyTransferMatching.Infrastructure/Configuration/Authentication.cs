namespace SFA.DAS.LevyTransferMatching.Infrastructure.Configuration
{
    public class Authentication
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string ResponseType { get; set; }
        public bool SaveTokens { get; set; }
        public string Scopes { get; set; }
        public string ChangeEmailUrl { get; set; }

        public string ChangeEmailLinkFormatted()
        {
            return BaseAddress.Replace("/identity", "") + string.Format(ChangeEmailUrl, ClientId);
        }

        public string ChangePasswordUrl { get; set; }
        public string ChangePasswordLinkFormatted()
        {
            return BaseAddress.Replace("/identity", "") + string.Format(ChangePasswordUrl, ClientId);
        }
        public ClaimIdentifierConfiguration ClaimIdentifierConfiguration { get; set; }
    }
}
