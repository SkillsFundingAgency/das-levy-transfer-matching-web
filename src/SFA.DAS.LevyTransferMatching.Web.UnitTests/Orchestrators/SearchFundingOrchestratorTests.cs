using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.SearchFundingService;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Models.SearchFunding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class SearchFundingOrchestratorTests
    {
        private SearchFundingOrchestrator _orchestrator;
        private Fixture _fixture;
        private Mock<ISearchFundingService> _searchFundingService;

        private List<OpportunityDto> _opportunityDtoList;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _searchFundingService = new Mock<ISearchFundingService>();

            _opportunityDtoList = _fixture.Create<List<OpportunityDto>>();
            _searchFundingService.Setup(x => x.GetAllOpportunities()).ReturnsAsync(_opportunityDtoList);

            _orchestrator = new SearchFundingOrchestrator(_searchFundingService.Object);
        }

        [Test]
        public async Task GetSearchFundingViewModel_Opportunities_Are_Populated()
        {
            var test = await _orchestrator.GetSearchFundingViewModel();

            Assert.AreEqual(test.Opportunities[0].EmployerName, _opportunityDtoList[0].DasAccountName);
            Assert.AreEqual(test.Opportunities[0].ReferenceNumber, _opportunityDtoList[0].EncodedPledgeId);
        }
    }
}
