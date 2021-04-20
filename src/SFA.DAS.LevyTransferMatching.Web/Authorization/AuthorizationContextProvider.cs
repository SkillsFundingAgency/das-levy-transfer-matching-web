using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.EmployerUserRoles.Context;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Authentication;

namespace SFA.DAS.LevyTransferMatching.Web.Authorization
{
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncodingService _encodingService;
        private readonly IAuthenticationService _authenticationService;

        public AuthorizationContextProvider(IHttpContextAccessor httpContextAccessor,
            IEncodingService encodingService,
            IAuthenticationService authenticationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _encodingService = encodingService;
            _authenticationService = authenticationService;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            var authorizationContext = new AuthorizationContext();
            var accountId = GetAccountId();
            var userRef = GetUserRef();

            CopyRouteValueToAuthorizationContextIfAvailable(authorizationContext, accountId, AuthorizationContextKeys.AccountId);

            if (accountId.HasValue && userRef.HasValue)
            {
                authorizationContext.AddEmployerUserRoleValues(accountId.Value, userRef.Value);
            }

            return authorizationContext;
        }

        private long? GetAccountId()
        {
            return GetAndDecodeValueIfExists(RouteValueKeys.EncodedAccountId, EncodingType.AccountId);
        }


        private Guid? GetUserRef()
        {
            if (!_authenticationService.IsUserAuthenticated())
            {
                return null;
            }

            if (!_authenticationService.TryGetUserClaimValue(ClaimIdentifierConfiguration.Id, out var idClaimValue))
            {
                throw new UnauthorizedAccessException($"Failed to get value for claim '{ClaimIdentifierConfiguration.Id}'");
            }

            if (!Guid.TryParse(idClaimValue, out var id))
            {
                throw new UnauthorizedAccessException($"Failed to parse value '{idClaimValue}' for claim '{ClaimIdentifierConfiguration.Id}'");
            }

            return id;
        }

        private long? GetAndDecodeValueIfExists(string keyName, EncodingType encodedType)
        {
            if (!TryGetValueFromHttpContext(keyName, out var encodedValue))
            {
                return null;
            }

            if (!_encodingService.TryDecode(encodedValue, encodedType, out var id))
            {
                throw new UnauthorizedAccessException($"Failed to decode '{keyName}' value '{encodedValue}' using encoding type '{encodedType}'");
            }

            return id;
        }

        private bool TryGetValueFromHttpContext(string key, out string value)
        {
            value = null;

            if (_httpContextAccessor.HttpContext.GetRouteData().Values.TryGetValue(key, out var routeValue))
            {
                value = (string)routeValue;
            }
            else if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue(key, out var queryStringValue))
            {
                value = queryStringValue;
            }
            else if (_httpContextAccessor.HttpContext.Request.HasFormContentType && _httpContextAccessor.HttpContext.Request.Form.TryGetValue(key, out var formValue))
            {
                value = formValue;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return true;
        }

        private void CopyRouteValueToAuthorizationContextIfAvailable<T>(IAuthorizationContext ctx, T? value, string name) where T : struct
        {
            if (value.HasValue)
            {
                ctx.Set(name, value.Value);
            }
        }
    }
}
