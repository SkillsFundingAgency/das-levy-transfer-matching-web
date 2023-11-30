using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.UnitTests.Services;

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
        var accountOwnerAccountIds = _fixture.CreateMany<string>();
        var accountTransactorAccountIds = _fixture.CreateMany<string>();

        var expectedAccountIds = accountOwnerAccountIds.Concat(accountTransactorAccountIds).Distinct();

        _mockClaimsIdentity
            .Setup(x => x.IsAuthenticated)
            .Returns(true);


        var accounts = accountOwnerAccountIds
            .Select(ownerAccountId => new EmployerUserAccountItem
                {AccountId = ownerAccountId, EmployerName = $"test - {ownerAccountId}", Role = "Owner"}).ToList();
        accounts.AddRange(accountTransactorAccountIds.Select(ownerAccountId => new EmployerUserAccountItem
            {AccountId = ownerAccountId, EmployerName = $"test - {ownerAccountId}", Role = "Transactor"}).ToList());


        _mockClaimsIdentity
            .Setup(x => x.FindFirst(It.Is<string>(y => y == ClaimIdentifierConfiguration.Account)))
            .Returns(new Claim(ClaimIdentifierConfiguration.Account, JsonConvert.SerializeObject(accounts.ToDictionary(k => k.AccountId))));


        // Act
        var actualAccountIds = _userService.GetUserOwnerTransactorAccountIds();

        // Assert
        CollectionAssert.AreEqual(expectedAccountIds, actualAccountIds);
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