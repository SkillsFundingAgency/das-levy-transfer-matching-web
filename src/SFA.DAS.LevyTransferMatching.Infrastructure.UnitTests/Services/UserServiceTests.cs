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
        public void GetUserOwnerTransactorAccountIds_UserAuthenticatedAndOwnerAndTransactorAccountIdClaimsPresent_ReturnsParsedList()
        {
            // Arrange
            var accountOwnerAccountIds = _fixture.CreateMany<long>();
            var accountTransactorAccountIds = accountOwnerAccountIds.Concat(_fixture.CreateMany<long>());

            var expectedAccountIds = accountOwnerAccountIds.Concat(accountTransactorAccountIds).Distinct();

            _mockClaimsIdentity
                .Setup(x => x.IsAuthenticated)
                .Returns(true);

            var accountOwnerClaims = accountOwnerAccountIds
                .Select(x => new Claim(ClaimIdentifierConfiguration.AccountOwner, x.ToString()));
            var accountTransactorClaims = accountTransactorAccountIds
                .Select(x => new Claim(ClaimIdentifierConfiguration.AccountTransactor, x.ToString()));

            _mockClaimsIdentity
                .Setup(x => x.FindAll(It.Is<string>(y => y == ClaimIdentifierConfiguration.AccountOwner)))
                .Returns(accountOwnerClaims);
            _mockClaimsIdentity
                .Setup(x => x.FindAll(It.Is<string>(y => y == ClaimIdentifierConfiguration.AccountTransactor)))
                .Returns(accountTransactorClaims);

            // Act
            var actualAccountIds = _userService.GetUserOwnerTransactorAccountIds();

            // Assert
            CollectionAssert.AreEqual(expectedAccountIds, actualAccountIds);
        }

        [Test]
        public void GetUserOwnerTransactorAccountIds_UserAuthenticatedAndOwnerAccountIdClaimsPresent_ReturnsParsedList()
        {
            // Arrange
            var accountOwnerAccountIds = _fixture.CreateMany<long>();

            _mockClaimsIdentity
                .Setup(x => x.IsAuthenticated)
                .Returns(true);

            var accountOwnerClaims = accountOwnerAccountIds
                .Select(x => new Claim(ClaimIdentifierConfiguration.AccountOwner, x.ToString()));

            _mockClaimsIdentity
                .Setup(x => x.FindAll(It.Is<string>(y => y == ClaimIdentifierConfiguration.AccountOwner)))
                .Returns(accountOwnerClaims);

            // Act
            var actualAccountIds = _userService.GetUserOwnerTransactorAccountIds();

            // Assert
            CollectionAssert.AreEqual(accountOwnerAccountIds, actualAccountIds);
        }

        [Test]
        public void GetUserOwnerTransactorAccountIds_UserAuthenticatedAndTransactorAccountIdClaimsPresent_ReturnsParsedList()
        {
            // Arrange
            var accountTransactorAccountIds = _fixture.CreateMany<long>();

            _mockClaimsIdentity
                .Setup(x => x.IsAuthenticated)
                .Returns(true);

            var accountTransactorClaims = accountTransactorAccountIds
                .Select(x => new Claim(ClaimIdentifierConfiguration.AccountTransactor, x.ToString()));

            _mockClaimsIdentity
                .Setup(x => x.FindAll(It.Is<string>(y => y == ClaimIdentifierConfiguration.AccountTransactor)))
                .Returns(accountTransactorClaims);

            // Act
            var actualAccountIds = _userService.GetUserOwnerTransactorAccountIds();

            // Assert
            CollectionAssert.AreEqual(accountTransactorAccountIds, actualAccountIds);
        }

        [Test]
        public void GetUserOwnerTransactorAccountIds_UserNotAuthenticated_ReturnsNull()
        {
            // Arrange
            _mockClaimsIdentity
                .Setup(x => x.IsAuthenticated)
                .Returns(false);

            // Act
            var actualAccountIds = _userService.GetUserOwnerTransactorAccountIds();

            // Assert
            Assert.IsNull(actualAccountIds);
        }

        [Test]
        public void GetUserOwnerTransactorAccountIds_UserAuthenticatedAndAccountClaimsNotPresent_ReturnsNull()
        {
            // Arrange
            _mockClaimsIdentity
                .Setup(x => x.IsAuthenticated)
                .Returns(true);

            // Act
            var actualAccountIds = _userService.GetUserOwnerTransactorAccountIds();

            // Assert
            Assert.IsNull(actualAccountIds);
        }
    }
}