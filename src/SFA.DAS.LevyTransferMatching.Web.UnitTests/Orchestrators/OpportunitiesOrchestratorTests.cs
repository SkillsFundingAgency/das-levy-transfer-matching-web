using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        private List<OpportunityDto> _opportunityDtoList;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _searchFundingService = new Mock<IOpportunitiesService>();

            _opportunityDtoList = _fixture.Create<List<OpportunityDto>>();
            _searchFundingService.Setup(x => x.GetAllOpportunities()).ReturnsAsync(_opportunityDtoList);

            _orchestrator = new OpportunitiesOrchestrator(_searchFundingService.Object);
        }

        [Test]
        public async Task GetSearchFundingViewModel_Opportunities_Are_Populated()
        {
            var test = await _orchestrator.GetIndexViewModel();

            Assert.AreEqual(test.Opportunities[0].EmployerName, _opportunityDtoList[0].DasAccountName);
            Assert.AreEqual(test.Opportunities[0].ReferenceNumber, _opportunityDtoList[0].EncodedPledgeId);
        }
    }
}
