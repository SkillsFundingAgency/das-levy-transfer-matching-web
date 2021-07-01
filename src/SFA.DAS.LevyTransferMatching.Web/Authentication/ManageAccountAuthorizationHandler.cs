using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Authorization;

namespace SFA.DAS.LevyTransferMatching.Web.Authentication
{
    public class ManageAccountAuthorizationHandler : AuthorizationHandler<ManageAccountRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ManageAccountAuthorizationHandler> _logger;
        private readonly IEncodingService _encodingService;

        public ManageAccountAuthorizationHandler(IHttpContextAccessor httpContextAccessor,
            ILogger<ManageAccountAuthorizationHandler> logger,
            IEncodingService encodingService)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _encodingService = encodingService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAccountRequirement requirement)
        {
            _logger.LogInformation("ManageAccountAuthorizationHandler invoked");

            var isAuthorized = await IsEmployerAuthorized(context);

            if (isAuthorized)
            {
                context.Succeed(requirement);
            }
        }

        private Task<bool> IsEmployerAuthorized(AuthorizationHandlerContext context)
        {
            if (!_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValueKeys.EncodedAccountId))
            {
                return Task.FromResult(false);
            }

            var accountIdFromUrl =
                _httpContextAccessor.HttpContext.Request.RouteValues[RouteValueKeys.EncodedAccountId].ToString();

            var decodedAccountId = _encodingService.Decode(accountIdFromUrl, EncodingType.AccountId);
            
            var userIdClaim = context.User.FindFirst(c => c.Type.Equals(ClaimIdentifierConfiguration.Id));
            if (userIdClaim?.Value == null)
            {
                return Task.FromResult(false);
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Task.FromResult(false);
            }

            var accountClaim = context.User.FindFirst(c =>
                c.Type.Equals(ClaimIdentifierConfiguration.Account) &&
                c.Value.Equals(decodedAccountId.ToString(), StringComparison.InvariantCultureIgnoreCase)
            );

            if (accountClaim?.Value != null)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}