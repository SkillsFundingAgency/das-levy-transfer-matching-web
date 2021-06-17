using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Models.SearchFunding;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers
{
    [TestFixture]
    public class SearchFundingControllerTests
    {
        private SearchFundingController _searchFundingController;
        private Mock<ISearchFundingOrchestrator> _orchestrator;

        [SetUp]
        public void SetUp()
        {
            _orchestrator = new Mock<ISearchFundingOrchestrator>();

            _searchFundingController = new SearchFundingController(_orchestrator.Object);
        }

        [Test]
        public async Task GET_Index_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            _orchestrator.Setup(x => x.GetSearchFundingViewModel()).ReturnsAsync(() => new SearchFundingViewModel());

            // Act
            var viewResult = await _searchFundingController.Index() as ViewResult;
            var indexViewModel = viewResult?.Model as SearchFundingViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(indexViewModel);
        }
    }
}
