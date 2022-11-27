using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService.Types;
using SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.StartupExtensions
{
    public class PostAuthenticationClaimsHandlerTests
    {
        private string _userId;
        private string _email;
        private string _emailNotMatching;
        private PostAuthenticationClaimsHandler _handler;
        private GetUserAccountsResponse _response;
        private Mock<IAccountUserService> _accountUserService;
        private Infrastructure.Configuration.FeatureToggles _configuration;
        private string _legacyId;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _response = fixture.Create<GetUserAccountsResponse>();
            _response.UserAccounts.First().Role = UserRole.Owner.ToString();
            _response.UserAccounts.Last().Role = UserRole.Transactor.ToString();
            _userId = fixture.Create<string>();
            _email = fixture.Create<string>();
            _legacyId = fixture.Create<string>();
            _emailNotMatching = fixture.Create<string>();

            _configuration = new Infrastructure.Configuration.FeatureToggles
            {
                UseGovSignIn = true
            };
            _accountUserService = new Mock<IAccountUserService>();
            _accountUserService.Setup(x => x.GetUserAccounts(_email, _userId)).ReturnsAsync(_response);
            _accountUserService.Setup(x => x.GetUserAccounts(_emailNotMatching, _userId)).ReturnsAsync(new GetUserAccountsResponse
            {
                UserAccounts = new List<EmployerIdentifier>(),
                EmployerUserId = _legacyId,
                FirstName = fixture.Create<string>(),
                LastName = fixture.Create<string>()
            });

            _handler = new PostAuthenticationClaimsHandler(_accountUserService.Object, _configuration);

        }
        [Test]
        public async Task Then_The_Claims_Are_Passed_To_The_Api_And_Id_DisplayName_Populated()
        {
            var tokenValidatedContext = ArrangeTokenValidatedContext(_userId, _email,_legacyId);
            
            var actual = (await _handler.GetClaims(tokenValidatedContext)).ToList();

            actual.First(c=>c.Type.Equals(ClaimIdentifierConfiguration.Id)).Value.Should().Be(_response.EmployerUserId);
            actual.First(c=>c.Type.Equals(ClaimIdentifierConfiguration.DisplayName)).Value.Should().Be(_response.FirstName + " " + _response.LastName);
        }

        [Test]
        public async Task Then_The_Accounts_Are_Populated_For_Owner_And_Transactor_Accounts()
        {
            var tokenValidatedContext = ArrangeTokenValidatedContext(_userId, _email,_legacyId);
            
            var actual = (await _handler.GetClaims(tokenValidatedContext)).ToList();

            var actualAccountClaims = actual.Where(c => c.Type.Equals(ClaimIdentifierConfiguration.AccountOwner)).Select(c => c.Value)
                .ToList();
            actualAccountClaims.Count.Should().Be(2);
            actualAccountClaims.Should().BeEquivalentTo(
                _response.UserAccounts
                    .Where(c => c.Role.Equals(UserRole.Owner.ToString()) ||
                                c.Role.Equals(UserRole.Transactor.ToString())).Select(c => c.AccountId).ToList());
        }

        [Test]
        public async Task Then_If_No_Response_From_Api_Null_Returned()
        {
            var tokenValidatedContext = ArrangeTokenValidatedContext(_userId, _emailNotMatching, _legacyId);
            
            var actual = (await _handler.GetClaims(tokenValidatedContext)).ToList();

            actual.Should().BeEmpty();
        }
        
        private TokenValidatedContext ArrangeTokenValidatedContext(string nameIdentifier, string emailAddress, string legacyId)
        {
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimIdentifierConfiguration.Id, legacyId),
                new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
                new Claim(ClaimTypes.Email, emailAddress)
            });
        
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(identity));
            return new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",","", typeof(TestAuthHandler)),
                new OpenIdConnectOptions(), Mock.Of<ClaimsPrincipal>(), new AuthenticationProperties())
            {
                Principal = claimsPrincipal
            };
        }
        private class TestAuthHandler : IAuthenticationHandler
        {
            public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
            {
                throw new NotImplementedException();
            }

            public Task<AuthenticateResult> AuthenticateAsync()
            {
                throw new NotImplementedException();
            }

            public Task ChallengeAsync(AuthenticationProperties? properties)
            {
                throw new NotImplementedException();
            }

            public Task ForbidAsync(AuthenticationProperties? properties)
            {
                throw new NotImplementedException();
            }
        }
    }
}