using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers
{
    [TestFixture]
    public class OpportunitiesControllerTests
    {
        private OpportunitiesController _opportunitiesController;
        private Fixture _fixture;
        private Mock<IOpportunitiesOrchestrator> _orchestrator;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _orchestrator = new Mock<IOpportunitiesOrchestrator>();

            _opportunitiesController = new OpportunitiesController(_orchestrator.Object);
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
            var detailRequest = _fixture.Create<DetailRequest>();
            var expectedDetailViewModel = _fixture.Create<DetailViewModel>();

            _orchestrator
                .Setup(x => x.GetDetailViewModel(It.Is<int>(y => y == detailRequest.PledgeId)))
                .ReturnsAsync(expectedDetailViewModel);

            // Act
            var viewResult = await _opportunitiesController.Detail(detailRequest) as ViewResult;
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
            var detailRequest = _fixture.Create<DetailRequest>();

            _orchestrator
                .Setup(x => x.GetDetailViewModel(It.Is<int>(y => y == detailRequest.PledgeId)))
                .ReturnsAsync((DetailViewModel)null);

            // Act
            var notFoundResult = await _opportunitiesController.Detail(detailRequest) as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
        }

        [Test]
        public void POST_Detail_No_Selected_Redirects_To_Index()
        {
            // Arrange
            string encodedPledgeId = _fixture.Create<string>();
            DetailPostRequest detailPostRequest = new DetailPostRequest()
            {
                EncodedPledgeId = encodedPledgeId,
                HasConfirmed = false,
            };

            // Assert
            var redirectToActionResult = _opportunitiesController.Detail(detailPostRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(redirectToActionResult.ActionName, nameof(OpportunitiesController.Index));
        }

        [Test]
        public void POST_Detail_Yes_Selected_Redirects_To_SelectAccount()
        {
            // Arrange
            string encodedPledgeId = _fixture.Create<string>();
            DetailPostRequest detailPostRequest = new DetailPostRequest()
            {
                EncodedPledgeId = encodedPledgeId,
                HasConfirmed = true,
            };

            // Assert
            var redirectToActionResult = _opportunitiesController.Detail(detailPostRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(redirectToActionResult.ActionName, nameof(OpportunitiesController.SelectAccount));
        }

        [Test]
        public async Task GET_SelectAccount_Redirects_To_Authorized_Apply_Path()
        {
            string encodedAccountId = _fixture.Create<string>();

            _orchestrator
                .Setup(x => x.GetUserEncodedAccountId())
                .ReturnsAsync(encodedAccountId);

            string encodedPledgeId = _fixture.Create<string>();

            var redirectToActionResult = await _opportunitiesController.SelectAccount(encodedPledgeId) as RedirectToActionResult;

            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(redirectToActionResult.ActionName, nameof(OpportunitiesController.Apply));
        }

        [Test]
        public async Task GET_MoreDetails_Returns_Expected_View()
        {
            var request = _fixture.Create<MoreDetailsRequest>();
            var expectedViewModel = _fixture.Create<MoreDetailsViewModel>();

            _orchestrator
                .Setup(x => x.GetMoreDetailsViewModel(request))
                .ReturnsAsync(expectedViewModel);

            var viewResult = await _opportunitiesController.MoreDetails(request) as ViewResult;
            var actualViewModel = viewResult.Model as MoreDetailsViewModel;

            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(actualViewModel);            
            Assert.AreEqual(expectedViewModel, actualViewModel);
            _orchestrator.Verify(x => x.GetMoreDetailsViewModel(request), Times.Once);
        }

        [Test]
        public async Task POST_MoreDetails_Redirects_To_Apply()
        {
            // Arrange
            var encodedPledgeId = _fixture.Create<string>();
            var encodedAccountId = _fixture.Create<string>();
            var cacheKey = _fixture.Create<Guid>();

            var request = new MoreDetailsPostRequest()
            {
                EncodedPledgeId = encodedPledgeId,
                EncodedAccountId = encodedAccountId,
                Details = _fixture.Create<string>(),
                CacheKey = cacheKey
            };

            _orchestrator.Setup(x => x.UpdateCacheItem(request));

            // Assert
            var redirectToActionResult = (await _opportunitiesController.MoreDetails(request)) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(redirectToActionResult.ActionName, nameof(OpportunitiesController.Apply));
            Assert.AreEqual(redirectToActionResult.RouteValues["EncodedPledgeId"], encodedPledgeId);
            Assert.AreEqual(redirectToActionResult.RouteValues["EncodedAccountId"], encodedAccountId);
            Assert.AreEqual(redirectToActionResult.RouteValues["CacheKey"], cacheKey);
            _orchestrator.Verify(x => x.UpdateCacheItem(request), Times.Once);
        }

        [Test]
        public async Task GET_ApplicationDetails_Returns_Expected_ViewModel()
        {
            var request = _fixture.Create<ApplicationDetailsRequest>();
            var expectedViewModel = _fixture.Create<ApplicationDetailsViewModel>();

            _orchestrator
                .Setup(x => x.GetApplicationViewModel(request))
                .ReturnsAsync(expectedViewModel);

            var viewResult = await _opportunitiesController.ApplicationDetails(request) as ViewResult;
            var actualViewModel = viewResult.Model as ApplicationDetailsViewModel;

            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(actualViewModel);
            Assert.AreEqual(expectedViewModel, actualViewModel);
            _orchestrator.Verify(x => x.GetApplicationViewModel(request), Times.Once);
        }

        [Test]
        public async Task POST_ApplicationDetails_Redirects_To_Apply()
        {
            // Arrange
            var encodedPledgeId = _fixture.Create<string>();
            var encodedAccountId = _fixture.Create<string>();
            var cacheKey = _fixture.Create<Guid>();

            var request = new ApplicationDetailsPostRequest()
            {
                EncodedPledgeId = encodedPledgeId,
                EncodedAccountId = encodedAccountId,
                CacheKey = cacheKey,
                HasTrainingProvider = true,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                NumberOfApprentices = 1,
                PledgeId = 1,
                SelectedStandardId = "ST_001",
                SelectedStandardTitle = "Test Standard Title"
            };

            _orchestrator.Setup(x => x.UpdateCacheItem(request));

            // Assert
            var redirectToActionResult = (await _opportunitiesController.ApplicationDetails(request)) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(redirectToActionResult.ActionName, nameof(OpportunitiesController.Apply));
            Assert.AreEqual(redirectToActionResult.RouteValues["EncodedPledgeId"], encodedPledgeId);
            Assert.AreEqual(redirectToActionResult.RouteValues["EncodedAccountId"], encodedAccountId);
            Assert.AreEqual(redirectToActionResult.RouteValues["CacheKey"], cacheKey);
            _orchestrator.Verify(x => x.UpdateCacheItem(request), Times.Once);
        }

        [Test]
        public async Task GET_Sector_Returns_Expected_ViewModel()
        {
            var request = _fixture.Create<SectorRequest>();
            var expectedViewModel = _fixture.Create<SectorViewModel>();

            _orchestrator
                .Setup(x => x.GetSectorViewModel(request))
                .ReturnsAsync(expectedViewModel);

            var viewResult = await _opportunitiesController.Sector(request) as ViewResult;
            var actualViewModel = viewResult.Model as SectorViewModel;

            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(actualViewModel);
            Assert.AreEqual(expectedViewModel, actualViewModel);
            _orchestrator.Verify(x => x.GetSectorViewModel(request), Times.Once);
        }

        [Test]
        public async Task POST_Sector_Redirects_To_Apply()
        {
            // Arrange
            var encodedPledgeId = _fixture.Create<string>();
            var encodedAccountId = _fixture.Create<string>();
            var cacheKey = _fixture.Create<Guid>();
            var validPostcode = "ST4 5NQ";

            var opportunitiesService = new Mock<IOpportunitiesService>();
            opportunitiesService.Setup(x => x.GetSector(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new GetSectorResponse { Location = "Valid" });
            var validator = new SectorPostRequestAsyncValidator(opportunitiesService.Object);

            var request = new SectorPostRequest
            {
                Sectors = new List<string> { "Sector" },
                Postcode = validPostcode,
                EncodedPledgeId = encodedPledgeId,
                EncodedAccountId = encodedAccountId,
                CacheKey = cacheKey
            };

            _orchestrator.Setup(x => x.UpdateCacheItem(request));

            // Assert
            var redirectToActionResult = (await _opportunitiesController.Sector(validator, request)) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(redirectToActionResult.ActionName, nameof(OpportunitiesController.Apply));
            Assert.AreEqual(redirectToActionResult.RouteValues["EncodedPledgeId"], encodedPledgeId);
            Assert.AreEqual(redirectToActionResult.RouteValues["EncodedAccountId"], encodedAccountId);
            Assert.AreEqual(redirectToActionResult.RouteValues["CacheKey"], cacheKey);
            _orchestrator.Verify(x => x.UpdateCacheItem(request), Times.Once);
        }

        [Test]
        public async Task POST_Sector_Redirects_To_Sector_When_Validation_Fails()
        {
            // Arrange
            var encodedPledgeId = _fixture.Create<string>();
            var encodedAccountId = _fixture.Create<string>();
            var cacheKey = _fixture.Create<Guid>();

            var opportunitiesService = new Mock<IOpportunitiesService>();
            opportunitiesService.Setup(x => x.GetSector(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new GetSectorResponse { Location = "" });
            var validator = new SectorPostRequestAsyncValidator(opportunitiesService.Object);

            var request = new SectorPostRequest
            {
                Sectors = new List<string>(),
                Postcode = "InvalidPostcode",
                EncodedPledgeId = encodedPledgeId,
                EncodedAccountId = encodedAccountId,
                CacheKey = cacheKey
            };

            _orchestrator.Setup(x => x.UpdateCacheItem(request));

            // Assert
            var redirectToActionResult = (await _opportunitiesController.Sector(validator, request)) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(redirectToActionResult.ActionName, nameof(OpportunitiesController.Sector));
        }
    }
}
