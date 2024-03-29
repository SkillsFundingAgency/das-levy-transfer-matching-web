using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;
using SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.StartupExtensions;

public class PostAuthenticationClaimsHandlerTests
{
    private string _userId;
    private string _email;
    private string _emailNotMatching;
    private string _emailSuspended;
    private PostAuthenticationClaimsHandler _handler;
    private EmployerUserAccounts _response;
    private Mock<IAccountUserService> _accountUserService;
    private Infrastructure.Configuration.FeatureToggles _configuration;
    private string _legacyId;
    private GetUserAccountsResponse _responseSuspended;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _response = fixture.Create<EmployerUserAccounts>();
        _response.EmployerAccounts.First().Role = UserRole.Owner.ToString();
        _response.EmployerAccounts.Last().Role = UserRole.Transactor.ToString();
        _response.IsSuspended = false;
        _responseSuspended = fixture.Create<GetUserAccountsResponse>();
        _responseSuspended.IsSuspended = true;
        _userId = fixture.Create<string>();
        _email = fixture.Create<string>();
        _legacyId = fixture.Create<string>();
        _emailNotMatching = fixture.Create<string>();
        _emailSuspended = fixture.Create<string>();

        _configuration = new Infrastructure.Configuration.FeatureToggles
        {
            UseGovSignIn = true
        };
        _accountUserService = new Mock<IAccountUserService>();
        _accountUserService.Setup(x => x.GetUserAccounts(_email, _userId)).ReturnsAsync(_response);
        _accountUserService.Setup(x => x.GetUserAccounts(_emailSuspended, _userId)).ReturnsAsync(_responseSuspended);
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
    public async Task Then_The_Accounts_Are_Populated()
    {
        var tokenValidatedContext = ArrangeTokenValidatedContext(_userId, _email,_legacyId);
            
        var actual = (await _handler.GetClaims(tokenValidatedContext)).ToList();

        var actualClaimValue = actual.First(c => c.Type.Equals(ClaimIdentifierConfiguration.Account)).Value;
        actual.FirstOrDefault(c => c.Type.Equals(ClaimTypes.AuthorizationDecision))?.Value.Should().BeNullOrEmpty();
        JsonConvert.SerializeObject(_response.EmployerAccounts.ToDictionary(k => k.AccountId)).Should().Be(actualClaimValue);
    }

    [Test]
    public async Task Then_If_No_Response_From_Api_Null_Returned()
    {
        var tokenValidatedContext = ArrangeTokenValidatedContext(_userId, _emailNotMatching, _legacyId);
            
        var actual = (await _handler.GetClaims(tokenValidatedContext)).ToList();

        actual.Should().BeEmpty();
    }
        
    [Test]
    public async Task Then_If_Suspended_Flag_Set_In_Response_From_Api_Claim_Set()
    {
        var tokenValidatedContext = ArrangeTokenValidatedContext(_userId, _emailSuspended, _legacyId);
            
        var actual = (await _handler.GetClaims(tokenValidatedContext)).ToList();

        actual.First(c=>c.Type.Equals(ClaimTypes.AuthorizationDecision)).Value.Should().Be("Suspended");
    }
    private static TokenValidatedContext ArrangeTokenValidatedContext(string nameIdentifier, string emailAddress, string legacyId)
    {
        var identity = new ClaimsIdentity(new List<Claim>
        {
            new(ClaimIdentifierConfiguration.Id, legacyId),
            new(ClaimTypes.NameIdentifier, nameIdentifier),
            new(ClaimTypes.Email, emailAddress)
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