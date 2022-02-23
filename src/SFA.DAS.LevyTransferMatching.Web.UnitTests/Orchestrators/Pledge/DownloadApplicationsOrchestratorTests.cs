using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators.Pledge;
using SFA.DAS.LevyTransferMatching.Web.Services;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators.Pledge
{
    [TestFixture]
    public class DownloadApplicationsOrchestratorTests
    {
        private readonly Fixture _fixture = new Fixture();
        private DownloadApplicationsOrchestrator _orchestrator;
        private Mock<ICsvHelperService> _csvService;
        private Mock<ICacheStorageService> _cacheService;
        private Mock<IPledgeService> _pledgeService;
        private Mock<IEncodingService> _encodingService;
        private readonly int _pledgeId = 1;

        [SetUp]
        public void Setup()
        {
            _csvService = new Mock<ICsvHelperService>();
            _cacheService = new Mock<ICacheStorageService>();
            _pledgeService = new Mock<IPledgeService>();
            _encodingService = new Mock<IEncodingService>();

            _orchestrator = new DownloadApplicationsOrchestrator(_cacheService.Object, _pledgeService.Object, _csvService.Object, _encodingService.Object);
        }

        [Test]
        public async Task GetPledgeApplicationsDownloadModel_Retrieves_ApplicationsFromService()
        {
            var accountId = _fixture.Create<int>();
            var getPledgeApplicationsResponse = _fixture.Create<GetApplicationsResponse>();
            _pledgeService.Setup(o =>
                o.GetApplications(It.Is<long>(l => l == accountId), It.Is<int>(p => p == _pledgeId))).ReturnsAsync(getPledgeApplicationsResponse);

            await _orchestrator.GetPledgeApplicationsDownloadModel(new ApplicationsRequest
            {
                AccountId = accountId,
                PledgeId = _pledgeId
            });

            _pledgeService.Verify(o => o.GetApplications(It.Is<long>(l => l == accountId), It.Is<int>(p => p == _pledgeId)), Times.Once);
        }
    }
}
