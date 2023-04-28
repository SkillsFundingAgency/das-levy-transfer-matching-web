using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService.Types;
using SFA.DAS.LevyTransferMatching.Web.Authentication;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Authorisation
{
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
            Assert.IsFalse(_context.HasSucceeded);
        }

        [Test]
        public async Task Then_The_Requirement_Succeeds_If_Handler_Returns_True()
        {

            _employerAccountAuthorizationHandler.Setup(x => x.IsEmployerAuthorized(_context, UserRole.Transactor))
                .ReturnsAsync(true);
            
            await _handler.HandleAsync(_context);
            Assert.IsTrue(_context.HasSucceeded);
        }
    }
}
