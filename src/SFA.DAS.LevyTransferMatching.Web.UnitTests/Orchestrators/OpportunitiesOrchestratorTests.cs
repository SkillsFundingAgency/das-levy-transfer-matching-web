using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class OpportunitiesOrchestratorTests
    {
        private OpportunitiesOrchestrator _orchestrator;
        private Fixture _fixture;
        private Mock<IOpportunitiesService> _searchFundingService;
        private Mock<ITagService> _tagService;
        private Mock<IEncodingService> _encodingService;

        private List<ReferenceDataItem> _sectors;
        private List<ReferenceDataItem> _levels;
        private List<ReferenceDataItem> _jobRoles;

        private List<OpportunityDto> _opportunityDtoList;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _searchFundingService = new Mock<IOpportunitiesService>();
			_tagService = new Mock<ITagService>();
            _encodingService = new Mock<IEncodingService>();

            _opportunityDtoList = _fixture.Create<List<OpportunityDto>>();
            _sectors = _fixture.Create<List<ReferenceDataItem>>();
            _levels = _fixture.Create<List<ReferenceDataItem>>();
            _jobRoles = _fixture.Create<List<ReferenceDataItem>>();
            
            _searchFundingService.Setup(x => x.GetAllOpportunities()).ReturnsAsync(_opportunityDtoList);
            _tagService.Setup(x => x.GetJobRoles()).ReturnsAsync(_jobRoles);
            _tagService.Setup(x => x.GetSectors()).ReturnsAsync(_sectors);
            _tagService.Setup(x => x.GetLevels()).ReturnsAsync(_levels);
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PledgeId)).Returns("test");

            _orchestrator = new OpportunitiesOrchestrator(_searchFundingService.Object, _tagService.Object, _encodingService.Object);
        }

        [Test]
        public async Task GetSearchFundingViewModel_Opportunities_Are_Populated()
        {
            var test = await _orchestrator.GetIndexViewModel();

            Assert.AreEqual(test.Opportunities[0].EmployerName, _opportunityDtoList[0].DasAccountName);
            Assert.AreEqual(test.Opportunities[0].ReferenceNumber, _opportunityDtoList[0].EncodedPledgeId);
            Assert.AreEqual(test.Opportunities[0].Amount, _opportunityDtoList[0].Amount);
            Assert.AreEqual(_opportunityDtoList[0].DasAccountName, test.Opportunities[0].EmployerName);
            Assert.AreEqual("test", test.Opportunities[0].ReferenceNumber);
        }
    }
}
