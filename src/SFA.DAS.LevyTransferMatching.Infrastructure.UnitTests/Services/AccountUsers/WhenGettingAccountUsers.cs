using SFA.DAS.GovUK.Auth.Employer;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.UnitTests.Services.AccountUsers;

public class WhenGettingAccountUsers
{
    private string _email;
    private string _userId;
    private GetUserAccountsResponse _response;
    private string _emailNotMatch;
    private AccountUserService _service;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _email = fixture.Create<string>() + "+1@email.com";
        _emailNotMatch = fixture.Create<string>() + "+1@email.com";
        _userId = fixture.Create<string>();
        _response = fixture.Create<GetUserAccountsResponse>();
        var httpMessageHandler = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonConvert.SerializeObject(_response)),
            StatusCode = HttpStatusCode.OK
        };
        var notFoundResponse = new HttpResponseMessage
        {
            Content = new StringContent(JsonConvert.SerializeObject(new GetUserAccountsResponse())),
            StatusCode = HttpStatusCode.OK
        };
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.RequestUri.Equals(new Uri($"https://tempuri.org/AccountUsers/{_userId}/accounts?email={WebUtility.UrlEncode(_email)}"))
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync((HttpRequestMessage _, CancellationToken _) => response);
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.RequestUri.Equals(new Uri($"https://tempuri.org/AccountUsers/{_userId}/accounts?email={WebUtility.UrlEncode(_emailNotMatch)}"))
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync((HttpRequestMessage _, CancellationToken _) => notFoundResponse);
            
        var client = new HttpClient(httpMessageHandler.Object);
        client.BaseAddress = new Uri("https://tempuri.org");

        _service = new AccountUserService(client);
    }
        
    [Test]
    public async Task Then_The_Api_Is_Called_And_Response_Returned_For_User()
    {
        var actual = await _service.GetUserAccounts(_userId, _email);

        actual.Should().BeEquivalentTo(_response);
    }

    [Test]
    public async Task Then_The_Api_Is_Called_And_Empty_Returned_For_Non_Matching_User()
    {
        var actual = await _service.GetUserAccounts(_userId, _emailNotMatch);

        actual.Should().BeEquivalentTo(new EmployerUserAccounts());
    }
}