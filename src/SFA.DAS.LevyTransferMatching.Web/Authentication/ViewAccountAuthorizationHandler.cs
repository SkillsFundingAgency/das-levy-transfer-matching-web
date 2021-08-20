using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Authorization;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Authentication
{
    public class ViewAccountAuthorizationHandler : AuthorizationHandler<ViewAccountRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ManageAccountAuthorizationHandler> _logger;
        private readonly IEncodingService _encodingService;

        public ViewAccountAuthorizationHandler(IHttpContextAccessor httpContextAccessor,
            ILogger<ManageAccountAuthorizationHandler> logger,
            IEncodingService encodingService)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _encodingService = encodingService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewAccountRequirement requirement)
        {
            var isAuthorized = await IsEmployerAuthorized(context);

            var userClaim = context.User.FindFirst(c => c.Type.Equals(ClaimIdentifierConfiguration.Id));
            var userId = string.IsNullOrWhiteSpace(userClaim?.Value) ? "unknown" : userClaim.Value;

            if (isAuthorized)
            {
                _logger.LogInformation($"ViewAccountRequirement met for user [{userId}]");
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation($"ViewAccountRequirement not met for user [{userId}]");
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
                (c.Type.Equals(ClaimIdentifierConfiguration.AccountViewer) ||
                c.Type.Equals(ClaimIdentifierConfiguration.AccountOwner) ||
                c.Type.Equals(ClaimIdentifierConfiguration.AccountTransactor)) &&
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
