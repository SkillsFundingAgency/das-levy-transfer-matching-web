using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.Authentication
{
    public class EmployerStubAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmployerStubAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder, clock)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, "10000001"),
                new Claim(ClaimIdentifierConfiguration.Id, "37e9761e-1dc9-42b7-9054-787794ad6442"),
                new Claim(ClaimIdentifierConfiguration.DisplayName, "Mega Corp Administrator"),
            };

            var identity = new ClaimsIdentity(claims, "Employer-stub");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Employer-stub");
            var result = AuthenticateResult.Success(ticket);
            _httpContextAccessor.HttpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, "10000001");
            return Task.FromResult(result);
        }
    }
}
