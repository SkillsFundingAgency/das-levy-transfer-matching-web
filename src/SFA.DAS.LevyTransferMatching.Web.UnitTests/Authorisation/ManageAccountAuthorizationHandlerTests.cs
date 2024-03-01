using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;
using SFA.DAS.LevyTransferMatching.Web.Authentication;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Authorisation;

[TestFixture]
public class WhenHandleRequirementAsync
{
    private ManageAccountAuthorizationHandler _handler;
    private AuthorizationHandlerContext _context;
    private Mock<IEmployerAccountAuthorizationHandler> _employerAccountAuthorizationHandler;


    [SetUp]
    public void SetUp()
    {
        _context = new AuthorizationHandlerContext(new[] {new ManageAccountRequirement()}, new ClaimsPrincipal(), "");
        _employerAccountAuthorizationHandler = new Mock<IEmployerAccountAuthorizationHandler>();
        _handler = new ManageAccountAuthorizationHandler(_employerAccountAuthorizationHandler.Object);
    }

    [Test]
    public async Task Then_The_Requirement_Fails_If_Handler_Returns_False()
    {
        _employerAccountAuthorizationHandler.Setup(x => x.IsEmployerAuthorized(_context, UserRole.Transactor))
            .ReturnsAsync(false);
            
        await _handler.HandleAsync(_context);
        Assert.That(_context.HasSucceeded, Is.False);
    }

    [Test]
    public async Task Then_The_Requirement_Succeeds_If_Handler_Returns_True()
    {
        _employerAccountAuthorizationHandler.Setup(x => x.IsEmployerAuthorized(_context, UserRole.Transactor))
            .ReturnsAsync(true);
            
        await _handler.HandleAsync(_context);
        Assert.That(_context.HasSucceeded, Is.True);
    }
}