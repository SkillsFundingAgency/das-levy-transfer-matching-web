using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Authorization;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Authorisation
{
    [TestFixture]
    public class WhenHandleRequirementAsync
    {
        private ManageAccountAuthorizationHandler _sut;
        private List<IAuthorizationRequirement> _requirements;
        private ClaimsPrincipal _user;
        private ClaimsIdentity _identity;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<IEncodingService> _encodingService;
        private Guid _userId;
        private List<Claim> _claims;
        private string _encodedAccountId;
        private long _decodedAccountId;
        private readonly Fixture _fixture = new Fixture();
        private AuthorizationHandlerContext _context;
        private DefaultHttpContext _httpContext;


        [SetUp]
        public void SetUp()
        {
            _userId = Guid.NewGuid();

            _requirements = new List<IAuthorizationRequirement>
            {
                new ManageAccountRequirement()
            };

            _encodedAccountId = _fixture.Create<string>();
            _decodedAccountId = _fixture.Create<long>();

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(_encodedAccountId, EncodingType.AccountId)).Returns(_decodedAccountId);

            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            var responseMock = new FeatureCollection();
            _httpContext = new DefaultHttpContext(responseMock);
            _httpContext.Request.RouteValues.Add("EncodedAccountId", _encodedAccountId);
            _httpContextAccessor.Setup(_ => _.HttpContext).Returns(_httpContext);

            _claims = new List<Claim>
            {
                new Claim(ClaimIdentifierConfiguration.Id, _userId.ToString()),
                new Claim(ClaimIdentifierConfiguration.Account, _decodedAccountId.ToString()),

            };
            _identity = new ClaimsIdentity(_claims);
            _user = new ClaimsPrincipal(_identity);

            _context = new AuthorizationHandlerContext(_requirements, _user, "");

            _sut = new ManageAccountAuthorizationHandler(_httpContextAccessor.Object, Mock.Of<ILogger<ManageAccountAuthorizationHandler>>(), _encodingService.Object);
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_route_does_not_contain_the_accountId()
        {
            _httpContext.Request.RouteValues.Remove(RouteValueKeys.EncodedAccountId);
            await _sut.HandleAsync(_context);
            Assert.IsFalse(_context.HasSucceeded);
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_user_does_not_have_a_userId_claim()
        {
            // arrange            
            _identity = new ClaimsIdentity(new List<Claim>());
            _user = new ClaimsPrincipal(_identity);
            _context = new AuthorizationHandlerContext(_requirements, _user, _context);

            await _sut.HandleAsync(_context);

            Assert.IsFalse(_context.HasSucceeded);
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_userid_claim_is_not_a_guid()
        {
            // arrange            
            _claims = new List<Claim>
            {
                new Claim(ClaimIdentifierConfiguration.Id, "NOT-A-GUID")
            };
            _identity = new ClaimsIdentity(_claims);
            _user = new ClaimsPrincipal(_identity);
            _context = new AuthorizationHandlerContext(_requirements, _user, _context);

            await _sut.HandleAsync(_context);

            Assert.IsFalse(_context.HasSucceeded);
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_accountId_claim_does_not_exist()
        {
            _claims.Remove(_claims.Find(c => c.Type == ClaimIdentifierConfiguration.Account));
            _identity = new ClaimsIdentity(_claims);
            _user = new ClaimsPrincipal(_identity);
            _context = new AuthorizationHandlerContext(_requirements, _user, _context);

            await _sut.HandleAsync(_context);

            Assert.IsFalse(_context.HasSucceeded);
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_accountId_claim_does_not_match_the_route_data()
        {
            _claims.Remove(_claims.Find(c => c.Type == ClaimIdentifierConfiguration.Account));
            _claims.Add(new Claim(ClaimIdentifierConfiguration.Account, Guid.NewGuid().ToString()));
            _identity = new ClaimsIdentity(_claims);
            _user = new ClaimsPrincipal(_identity);
            _context = new AuthorizationHandlerContext(_requirements, _user, _context);

            await _sut.HandleAsync(_context);
            
            Assert.IsFalse(_context.HasSucceeded);
        }

        [Test]
        public async Task Then_the_requirement_succeeds_if_the_route_accountid_matches_the_account_claim()
        {
            await _sut.HandleAsync(_context);
            Assert.IsTrue(_context.HasSucceeded);
        }
    }
}
