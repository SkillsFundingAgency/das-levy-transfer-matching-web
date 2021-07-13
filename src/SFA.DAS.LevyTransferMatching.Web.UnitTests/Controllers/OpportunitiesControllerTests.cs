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
        public async Task GET_ContactDetails_Returns_View_With_Expected_ViewModel()
        {
            // Arrange
            ContactDetailsRequest contactDetailsRequest = _fixture.Create<ContactDetailsRequest>();

            _orchestrator.Setup(x => x.GetContactDetailsViewModel(It.Is<ContactDetailsRequest>(y => y == contactDetailsRequest))).ReturnsAsync(() => new ContactDetailsViewModel());

            // Act
            var viewResult = await _opportunitiesController.ContactDetails(contactDetailsRequest) as ViewResult;
            var contactDetailsViewModel = viewResult?.Model as ContactDetailsViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(contactDetailsViewModel);
        }

        [Test]
        public async Task POST_ContactDetails_Cache_Updated_And_Redirects_To_Apply()
        {
            // Arrange
            string encodedAccountId = _fixture.Create<string>();
            string encodedPledgeId = _fixture.Create<string>();
            Guid cacheKey = _fixture.Create<Guid>();
            ContactDetailsPostRequest contactDetailsPostRequest = new ContactDetailsPostRequest()
            {
                EncodedAccountId = encodedAccountId,
                EncodedPledgeId = encodedPledgeId,
                CacheKey = cacheKey,
            };

            bool cacheUpdated = false;
            _orchestrator
                .Setup(x => x.UpdateCacheItem(It.Is<ContactDetailsPostRequest>(y => y == contactDetailsPostRequest)))
                .Callback(() =>
                {
                    cacheUpdated = true;
                });

            // Assert
            var redirectToActionResult = await _opportunitiesController.ContactDetails(contactDetailsPostRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(redirectToActionResult.ActionName, nameof(OpportunitiesController.Apply));
            Assert.IsTrue(cacheUpdated);
            Assert.AreEqual(redirectToActionResult.RouteValues["encodedAccountId"], encodedAccountId);
            Assert.AreEqual(redirectToActionResult.RouteValues["encodedPledgeId"], encodedPledgeId);
            Assert.AreEqual(redirectToActionResult.RouteValues["cacheKey"], cacheKey);
        }
    }
}
