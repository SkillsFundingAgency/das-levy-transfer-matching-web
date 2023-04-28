using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService.Types;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Authorization;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Authorisation
{
    public class EmployerAccountAuthorizationHandlerTests
    {
        [Test]
        [MoqInlineAutoData(UserRole.Owner, true, "Owner")]
        [MoqInlineAutoData(UserRole.Transactor, true, "Owner")]
        [MoqInlineAutoData(UserRole.Viewer, true, "Owner")]
        [MoqInlineAutoData(UserRole.Owner, false, "Transactor")]
        [MoqInlineAutoData(UserRole.Transactor, true, "Transactor")]
        [MoqInlineAutoData(UserRole.Viewer, true, "Transactor")]
        [MoqInlineAutoData(UserRole.Owner, false, "Viewer")]
        [MoqInlineAutoData(UserRole.Transactor, false, "Viewer")]
        [MoqInlineAutoData(UserRole.Viewer, true, "Viewer")]
        [MoqInlineAutoData(UserRole.Viewer, false, "noRole")]
        public async Task Then_Returns_True_If_Employer_Is_Authorized_For_Owner_Role_And_Owner(
            UserRole minimumRole,
            bool accessResult,
            string roleOnClaims,
            EmployerIdentifier employerIdentifier,
            ManageAccountRequirement ownerRequirement,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            EmployerAccountAuthorizationHandler authorizationHandler)
        {
            //Arrange
            employerIdentifier.Role = roleOnClaims;
            employerIdentifier.AccountId = employerIdentifier.AccountId.ToUpper();
            var employerAccounts = new Dictionary<string, EmployerIdentifier>{{employerIdentifier.AccountId, employerIdentifier}};
            var claim = new Claim(ClaimIdentifierConfiguration.Account, JsonConvert.SerializeObject(employerAccounts));
            var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {claim})});
            var context = new AuthorizationHandlerContext(new [] {ownerRequirement}, claimsPrinciple, null);
            var httpContext = new DefaultHttpContext(new FeatureCollection());
            httpContext.Request.RouteValues.Add(RouteValueKeys.EncodedAccountId,employerIdentifier.AccountId);
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            
            //Act
            var actual = await authorizationHandler.IsEmployerAuthorized(context, minimumRole);

            //Assert
            actual.Should().Be(accessResult);
        }
        
        [Test, MoqAutoData]
        public async Task Then_Returns_False_If_Employer_Is_Not_Authorized(
            string accountId,
            EmployerIdentifier employerIdentifier,
            ManageAccountRequirement ownerRequirement,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            EmployerAccountAuthorizationHandler authorizationHandler)
        {
            //Arrange
            employerIdentifier.Role = "Owner";
            employerIdentifier.AccountId = employerIdentifier.AccountId.ToUpper();
            var employerAccounts = new Dictionary<string, EmployerIdentifier>{{employerIdentifier.AccountId, employerIdentifier}};
            var claim = new Claim(ClaimIdentifierConfiguration.Account, JsonConvert.SerializeObject(employerAccounts));
            var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {claim})});
            var context = new AuthorizationHandlerContext(new[] {ownerRequirement}, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContext.Request.RouteValues.Add(RouteValueKeys.EncodedAccountId,accountId.ToUpper());
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            //Act
            var actual = await authorizationHandler.IsEmployerAuthorized(context, UserRole.Owner);

            //Assert
            actual.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public async Task Then_If_Not_In_Context_Claims_EmployerAccountService_Checked_And_True_Returned_If_Exists(
            string accountId,
            string userId,
            string email,
            EmployerIdentifier employerIdentifier,
            ManageAccountRequirement ownerRequirement,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            [Frozen] Mock<IAccountUserService> employerAccountService,
            [Frozen] Mock<IConfiguration> configuration,
            EmployerAccountAuthorizationHandler authorizationHandler)
        {
            //Arrange
            configuration.Setup(x=>x["UseGovSignIn"]).Returns("false");
            employerIdentifier.AccountId = accountId.ToUpper();
            employerIdentifier.Role = "Owner";
            employerAccountService.Setup(x => x.GetUserAccounts(userId, email))
                .ReturnsAsync(new GetUserAccountsResponse
                {
                    UserAccounts = new List<EmployerIdentifier>{ employerIdentifier }
                });
            
            var userClaim = new Claim(ClaimIdentifierConfiguration.Id, userId);
            var employerAccounts = new Dictionary<string, EmployerIdentifier>{{employerIdentifier.AccountId, employerIdentifier}};
            var employerAccountClaim = new Claim(ClaimIdentifierConfiguration.Account, JsonConvert.SerializeObject(employerAccounts));
            var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {employerAccountClaim, userClaim, new Claim(ClaimTypes.Email, email)})});
            var context = new AuthorizationHandlerContext(new[] {ownerRequirement}, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContext.Request.RouteValues.Add(RouteValueKeys.EncodedAccountId, accountId.ToUpper());
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            
            //Act
            var actual = await authorizationHandler.IsEmployerAuthorized(context, UserRole.Owner);

            //Assert
            actual.Should().BeTrue();
            
        }

        [Test, MoqAutoData]
        public async Task Then_If_Not_In_Context_Claims_EmployerAccountService_Checked_And_True_Returned_If_Exists_For_GovSignIn(
            string accountId,
            string userId,
            string email,
            EmployerIdentifier employerIdentifier,
            ManageAccountRequirement ownerRequirement,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            [Frozen] Mock<IAccountUserService> employerAccountService,
            [Frozen] Mock<IConfiguration> configuration,
            EmployerAccountAuthorizationHandler authorizationHandler)
        {
            //Arrange
            configuration.Setup(x=>x["UseGovSignIn"]).Returns("true");
            employerIdentifier.AccountId = accountId.ToUpper();
            employerIdentifier.Role = "Owner";
            employerAccountService.Setup(x => x.GetUserAccounts(userId, email))
                .ReturnsAsync(new GetUserAccountsResponse
                {
                    UserAccounts = new List<EmployerIdentifier>{ employerIdentifier }
                });
            
            var userClaim = new Claim(ClaimTypes.NameIdentifier, userId);
            var employerAccounts = new Dictionary<string, EmployerIdentifier>{{employerIdentifier.AccountId, employerIdentifier}};
            var employerAccountClaim = new Claim(ClaimIdentifierConfiguration.Account, JsonConvert.SerializeObject(employerAccounts));
            var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {employerAccountClaim, userClaim, new Claim(ClaimTypes.Email, email)})});
            var context = new AuthorizationHandlerContext(new[] {ownerRequirement}, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContext.Request.RouteValues.Add(RouteValueKeys.EncodedAccountId,accountId.ToUpper());
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            
            //Act
            var actual = await authorizationHandler.IsEmployerAuthorized(context, UserRole.Owner);

            //Assert
            actual.Should().BeTrue();
            
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_Not_In_Context_Claims_EmployerAccountService_Checked_And_False_Returned_If_Not_Exists(
            string accountId,
            string userId,
            EmployerIdentifier employerIdentifier,
            ManageAccountRequirement ownerRequirement,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            [Frozen] Mock<IAccountUserService> employerAccountService,
            [Frozen] Mock<IConfiguration> configuration,
            EmployerAccountAuthorizationHandler authorizationHandler)
        {
            //Arrange
            employerIdentifier.AccountId = employerIdentifier.AccountId.ToUpper();
            employerIdentifier.Role = "Owner";
            employerAccountService.Setup(x => x.GetUserAccounts(userId,""))
                .ReturnsAsync(new GetUserAccountsResponse
                {
                    UserAccounts = new List<EmployerIdentifier>{ employerIdentifier }
                });
            
            var userClaim = new Claim(ClaimIdentifierConfiguration.Id, userId);
            var employerAccounts = new Dictionary<string, EmployerIdentifier>{{employerIdentifier.AccountId, employerIdentifier}};
            var employerAccountClaim = new Claim(ClaimIdentifierConfiguration.Account, JsonConvert.SerializeObject(employerAccounts));
            var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {employerAccountClaim, userClaim})});
            var context = new AuthorizationHandlerContext(new[] {ownerRequirement}, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContext.Request.RouteValues.Add(RouteValueKeys.EncodedAccountId,accountId.ToUpper());
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            
            //Act
            var actual = await authorizationHandler.IsEmployerAuthorized(context, UserRole.Owner);

            //Assert
            actual.Should().BeFalse();
        }
        
        [Test, MoqAutoData]
        public async Task Then_Returns_False_If_AccountId_Not_In_Url(
            EmployerIdentifier employerIdentifier,
            ManageAccountRequirement ownerRequirement,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            EmployerAccountAuthorizationHandler authorizationHandler)
        {
            //Arrange
            employerIdentifier.Role = "Owner";
            employerIdentifier.AccountId = employerIdentifier.AccountId.ToUpper();
            var employerAccounts = new Dictionary<string, EmployerIdentifier>{{employerIdentifier.AccountId, employerIdentifier}};
            var claim = new Claim(ClaimIdentifierConfiguration.Account, JsonConvert.SerializeObject(employerAccounts));
            var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {claim})});
            var context = new AuthorizationHandlerContext(new[] {ownerRequirement}, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContext.Request.RouteValues.Clear();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            
            //Act
            var actual = await authorizationHandler.IsEmployerAuthorized(context, UserRole.Viewer);

            //Assert
            actual.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_False_If_No_Matching_AccountIdentifier_Claim_Found(
            EmployerIdentifier employerIdentifier,
            ManageAccountRequirement ownerRequirement,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            EmployerAccountAuthorizationHandler authorizationHandler)
        {
            //Arrange
            employerIdentifier.Role = "Viewer-Owner-Transactor";
            employerIdentifier.AccountId = employerIdentifier.AccountId.ToUpper();
            var employerAccounts = new Dictionary<string, EmployerIdentifier>{{employerIdentifier.AccountId, employerIdentifier}};
            var claim = new Claim("SomeOtherClaim", JsonConvert.SerializeObject(employerAccounts));
            var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {claim})});
            var context = new AuthorizationHandlerContext(new[] {ownerRequirement}, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContext.Request.RouteValues.Add(RouteValueKeys.EncodedAccountId,employerIdentifier.AccountId);
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            
            //Act
            var actual = await authorizationHandler.IsEmployerAuthorized(context, UserRole.Viewer);

            //Assert
            actual.Should().BeFalse();
        }
        
        [Test, MoqAutoData]
        public async Task Then_Returns_False_If_No_Matching_NameIdentifier_Claim_Found_For_GovSignIn(
            EmployerIdentifier employerIdentifier,
            ManageAccountRequirement ownerRequirement,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            [Frozen] Mock<IConfiguration> forecastingConfiguration,
            EmployerAccountAuthorizationHandler authorizationHandler)
        {
            //Arrange
            forecastingConfiguration.Setup(x=>x["UseGovSignIn"]).Returns("true");
            employerIdentifier.Role = "Viewer-Owner-Transactor";
            employerIdentifier.AccountId = employerIdentifier.AccountId.ToUpper();
            var employerAccounts = new Dictionary<string, EmployerIdentifier>{{employerIdentifier.AccountId, employerIdentifier}};
            var claim = new Claim("SomeOtherClaim", JsonConvert.SerializeObject(employerAccounts));
            var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {claim})});
            var context = new AuthorizationHandlerContext(new[] {ownerRequirement}, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContext.Request.RouteValues.Add(RouteValueKeys.EncodedAccountId,employerIdentifier.AccountId);
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            
            //Act
            var actual = await authorizationHandler.IsEmployerAuthorized(context, UserRole.Viewer);

            //Assert
            actual.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_False_If_The_Claim_Cannot_Be_Deserialized(
            EmployerIdentifier employerIdentifier,
            ManageAccountRequirement ownerRequirement,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            EmployerAccountAuthorizationHandler authorizationHandler)
        {
            //Arrange
            employerIdentifier.Role = "Owner";
            employerIdentifier.AccountId = employerIdentifier.AccountId.ToUpper();
            var claim = new Claim(ClaimIdentifierConfiguration.Account, JsonConvert.SerializeObject(employerIdentifier));
            var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {claim})});
            var context = new AuthorizationHandlerContext(new[] {ownerRequirement}, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContext.Request.RouteValues.Add(RouteValueKeys.EncodedAccountId,employerIdentifier.AccountId);
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            
            //Act
            var actual = await authorizationHandler.IsEmployerAuthorized(context, UserRole.Owner);

            //Assert
            actual.Should().BeFalse();
        }
    }
}