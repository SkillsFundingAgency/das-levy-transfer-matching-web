using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.UnitTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Fixture _fixture;
        private Mock<ClaimsIdentity> _mockClaimsIdentity;
        private UserService _userService;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            Mock<IHttpContextAccessor> mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _mockClaimsIdentity = new Mock<ClaimsIdentity>();

            mockHttpContextAccessor
                .Setup(x => x.HttpContext.User.Identity)
                .Returns(_mockClaimsIdentity.Object);

            _userService = new UserService(mockHttpContextAccessor.Object);
        }

        [Test]
        public void GetUserAccountIds_UserAuthenticatedAndAccountClaimsPresent_ReturnsParsedList()
        {
            // Arrange
            var expectedAccountIds = _fixture.CreateMany<long>();

            _mockClaimsIdentity
                .Setup(x => x.IsAuthenticated)
                .Returns(true);

            var accountClaims = expectedAccountIds
                .Select(x => new Claim(ClaimIdentifierConfiguration.Account, x.ToString()));
            
            _mockClaimsIdentity
                .Setup(x => x.FindAll(It.Is<string>(y => y == ClaimIdentifierConfiguration.Account)))
                .Returns(accountClaims);

            // Act
            var actualAccountIds = _userService.GetUserAccountIds();

            // Assert
            CollectionAssert.AreEqual(expectedAccountIds, actualAccountIds);
        }

        [Test]
        public void GetUserAccountIds_UserNotAuthenticated_ReturnsNull()
        {
            // Arrange
            _mockClaimsIdentity
                .Setup(x => x.IsAuthenticated)
                .Returns(false);

            // Act
            var actualAccountIds = _userService.GetUserAccountIds();

            // Assert
            Assert.IsNull(actualAccountIds);
        }

        [Test]
        public void GetUserAccountIds_UserAuthenticatedAndAccountClaimsNotPresent_ReturnsNull()
        {
            // Arrange
            _mockClaimsIdentity
                .Setup(x => x.IsAuthenticated)
                .Returns(true);

            IEnumerable<Claim> claims = null;

            _mockClaimsIdentity
                .Setup(x => x.FindAll(It.Is<string>(y => y == ClaimIdentifierConfiguration.Account)))
                .Returns(claims);

            // Act
            var actualAccountIds = _userService.GetUserAccountIds();

            // Assert
            Assert.IsNull(actualAccountIds);
        }
    }
}