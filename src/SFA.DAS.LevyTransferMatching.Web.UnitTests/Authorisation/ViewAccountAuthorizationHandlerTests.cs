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
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService.Types;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Authorisation
{
    [TestFixture]
    public class ViewAccountAuthorizationHandlerTests
    {
        private ViewAccountAuthorizationHandler _handler;
        private AuthorizationHandlerContext _context;
        private Mock<IEmployerAccountAuthorizationHandler> _employerAccountAuthorizationHandler;


        [SetUp]
        public void SetUp()
        {
            _context = new AuthorizationHandlerContext(new[] {new ViewAccountRequirement()}, new ClaimsPrincipal(), "");
            _employerAccountAuthorizationHandler = new Mock<IEmployerAccountAuthorizationHandler>();
            _handler = new ViewAccountAuthorizationHandler(_employerAccountAuthorizationHandler.Object);
        }

        
        [Test]
        public async Task Then_The_Requirement_Fails_If_Handler_Returns_False()
        {

            _employerAccountAuthorizationHandler.Setup(x => x.IsEmployerAuthorized(_context, UserRole.Viewer))
                .ReturnsAsync(false);
            
            await _handler.HandleAsync(_context);
            Assert.That(_context.HasSucceeded, Is.False);
        }

        [Test]
        public async Task Then_The_Requirement_Succeeds_If_Handler_Returns_True()
        {

            _employerAccountAuthorizationHandler.Setup(x => x.IsEmployerAuthorized(_context, UserRole.Viewer))
                .ReturnsAsync(true);
            
            await _handler.HandleAsync(_context);
            Assert.That(_context.HasSucceeded, Is.True);
        }
    }
}
