using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class OpportunitiesOrchestratorTests
    {
        private OpportunitiesOrchestrator _orchestrator;
        private Fixture _fixture;
        private Mock<IOpportunitiesService> _searchFundingService;
        private Mock<IEncodingService> _encodingService;

        private List<OpportunityDto> _opportunityDtoList;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _searchFundingService = new Mock<IOpportunitiesService>();
            _encodingService = new Mock<IEncodingService>();

            _opportunityDtoList = _fixture.Create<List<OpportunityDto>>();
            _searchFundingService.Setup(x => x.GetAllOpportunities()).ReturnsAsync(_opportunityDtoList);
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PledgeId)).Returns("test");

            _orchestrator = new OpportunitiesOrchestrator(_searchFundingService.Object, _encodingService.Object);
        }

        [Test]
        public async Task GetSearchFundingViewModel_Opportunities_Are_Populated()
        {
            var test = await _orchestrator.GetIndexViewModel();

            Assert.AreEqual(_opportunityDtoList[0].DasAccountName, test.Opportunities[0].EmployerName);
            Assert.AreEqual("test", test.Opportunities[0].ReferenceNumber);
        }
    }
}
