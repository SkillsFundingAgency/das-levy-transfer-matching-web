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
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class OpportunitiesOrchestratorTests
    {
        private OpportunitiesOrchestrator _orchestrator;
        private Fixture _fixture;
        private Mock<ISearchFundingService> _searchFundingService;
        private Mock<ITagService> _tagService;

        private List<Tag> _sectors;
        private List<Tag> _levels;
        private List<Tag> _jobRoles;

        private List<OpportunityDto> _opportunityDtoList;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _searchFundingService = new Mock<ISearchFundingService>();
            _tagService = new Mock<ITagService>();

            _opportunityDtoList = _fixture.Create<List<OpportunityDto>>();
            _sectors = _fixture.Create<List<Tag>>();
            _levels = _fixture.Create<List<Tag>>();
            _jobRoles = _fixture.Create<List<Tag>>();
            
            _searchFundingService.Setup(x => x.GetAllOpportunities()).ReturnsAsync(_opportunityDtoList);
            _tagService.Setup(x => x.GetJobRoles()).ReturnsAsync(_jobRoles);
            _tagService.Setup(x => x.GetSectors()).ReturnsAsync(_sectors);
            _tagService.Setup(x => x.GetLevels()).ReturnsAsync(_levels);

            _orchestrator = new OpportunitiesOrchestrator(_searchFundingService.Object, _tagService.Object);
        }

        [Test]
        public async Task GetSearchFundingViewModel_Opportunities_Are_Populated()
        {
            var test = await _orchestrator.GetIndexViewModel();

            Assert.AreEqual(test.Opportunities[0].EmployerName, _opportunityDtoList[0].DasAccountName);
            Assert.AreEqual(test.Opportunities[0].ReferenceNumber, _opportunityDtoList[0].EncodedPledgeId);
            Assert.AreEqual(test.Opportunities[0].Amount, _opportunityDtoList[0].Amount);
        }
    }
}
