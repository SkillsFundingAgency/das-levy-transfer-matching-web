using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Data;
using SFA.DAS.LevyTransferMatching.Web.Authentication;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers
{
    [TestFixture]
    public class OpportunitiesControllerTests
    {
        private OpportunitiesController _opportunitiesController;
        private Fixture _fixture;
        private Mock<IAuthenticationService> _authenticationService;
        private Mock<IOpportunitiesOrchestrator> _orchestrator;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _authenticationService = new Mock<IAuthenticationService>();
            _orchestrator = new Mock<IOpportunitiesOrchestrator>();

            _opportunitiesController = new OpportunitiesController(_authenticationService.Object, _orchestrator.Object);
        }

        [Test]
        public async Task GET_Index_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            _orchestrator.Setup(x => x.GetIndexViewModel()).ReturnsAsync(() => new IndexViewModel());

            // Act
            var viewResult = await _opportunitiesController.Index() as ViewResult;
            var indexViewModel = viewResult?.Model as IndexViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(indexViewModel);
        }

        [Test]
        public async Task GET_Detail_Opportunity_Exists_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var encodedId = _fixture.Create<string>();
            var expectedDetailViewModel = _fixture.Create<DetailViewModel>();

            _orchestrator
                .Setup(x => x.GetDetailViewModel(It.Is<string>(y => y == encodedId)))
                .ReturnsAsync(expectedDetailViewModel);

            // Act
            var viewResult = await _opportunitiesController.Detail(encodedId) as ViewResult;
            var actualDetailViewModel = viewResult?.Model as DetailViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.IsNotNull(actualDetailViewModel);
            Assert.AreEqual(expectedDetailViewModel, actualDetailViewModel);
        }
        
        [Test]
        public async Task GET_Detail_Opportunity_Doesnt_Exist_Returns_404()
        {
            // Arrange
            var encodedId = _fixture.Create<string>();

            _orchestrator
                .Setup(x => x.GetDetailViewModel(It.Is<string>(y => y == encodedId)))
                .ReturnsAsync((DetailViewModel)null);

            // Act
            var notFoundResult = await _opportunitiesController.Detail(encodedId) as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
        }

        [Test]
        public void POST_ConfirmOpportunitySelection_No_Confirmation_Throws_DataException()
        {
            // Arrange
            string encodedId = _fixture.Create<string>();
            DetailPostRequest detailPostRequest = new DetailPostRequest();

            // Assert
            Assert.Throws<DataException>(() =>
            {
                // Act
                _opportunitiesController.ConfirmOpportunitySelection(encodedId, detailPostRequest);
            });
        }

        [Test]
        public void POST_ConfirmOpportunitySelection_No_Selected_Redirects_To_Index()
        {
            // Arrange
            string encodedId = _fixture.Create<string>();
            DetailPostRequest detailPostRequest = new DetailPostRequest()
            {
                HasConfirmed = false,
            };

            // Assert
            var redirectResult = _opportunitiesController.ConfirmOpportunitySelection(encodedId, detailPostRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(redirectResult.ActionName, nameof(OpportunitiesController.Index));
        }

        [Test]
        public void POST_ConfirmOpportunitySelection_Yes_Selected_Redirects_To_RedirectToApply()
        {
            // Arrange
            string encodedId = _fixture.Create<string>();
            DetailPostRequest detailPostRequest = new DetailPostRequest()
            {
                HasConfirmed = true,
            };

            // Assert
            var redirectResult = _opportunitiesController.ConfirmOpportunitySelection(encodedId, detailPostRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(redirectResult.ActionName, nameof(OpportunitiesController.RedirectToApply));
        }
    }
}
