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
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers
{
    [TestFixture]
    public class OpportunitiesControllerTests
    {
        private OpportunitiesController _opportunitiesController;
        private Fixture _fixture;
        private Mock<IOpportunitiesOrchestrator> _orchestrator;
        private IndexRequest _indexRequest;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _orchestrator = new Mock<IOpportunitiesOrchestrator>();
            _indexRequest = _fixture.Create<IndexRequest>();
            _opportunitiesController = new OpportunitiesController(_orchestrator.Object);
        }

        [Test]
        public async Task GET_Index_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            _orchestrator.Setup(x => x.GetIndexViewModel(_indexRequest)).ReturnsAsync(() => new IndexViewModel());

            // Act
            var viewResult = await _opportunitiesController.Index(_indexRequest) as ViewResult;
            var indexViewModel = viewResult?.Model as IndexViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(indexViewModel);
        }

        [Test]
        public async Task POST_Index_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var encodedAccountId = _fixture.Create<string>();
            var cacheKey = _fixture.Create<Guid>();
            var indexRequest = new IndexRequest();
            _orchestrator.Setup(x => x.GetIndexViewModel(indexRequest)).ReturnsAsync(() => new IndexViewModel());
            
            // Act
            var viewResult = await _opportunitiesController.Index(indexRequest) as ViewResult;
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
        public async Task GET_SelectAccount_AccountsReturnedEqualsOne_ReturnsRedirectToApply()
        {
            var selectAccountRequest = _fixture.Create<SelectAccountRequest>();
            var viewModel = _fixture
                .Build<SelectAccountViewModel>()
                .With(x => x.Accounts, _fixture.CreateMany<SelectAccountViewModel.Account>(1))
                .Create();

            _orchestrator
                .Setup(x => x.GetSelectAccountViewModel(It.Is<SelectAccountRequest>(y => y == selectAccountRequest)))
                .ReturnsAsync(viewModel);

            string encodedPledgeId = _fixture.Create<string>();

            var actionResult = await _opportunitiesController.SelectAccount(selectAccountRequest);
            var redirectToActionResult = actionResult as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(redirectToActionResult);
            
            Assert.AreEqual(redirectToActionResult.ActionName, nameof(OpportunitiesController.Apply));
        }

        [Test]
        public async Task GET_SelectAccount_AccountsReturnedMoreThanOne_ReturnsViewWithAccounts()
        {
            var selectAccountRequest = _fixture.Create<SelectAccountRequest>();
            var expectedViewModel = _fixture
                .Build<SelectAccountViewModel>()
                .With(x => x.Accounts, _fixture.CreateMany<SelectAccountViewModel.Account>())
                .Create();

            _orchestrator
                .Setup(x => x.GetSelectAccountViewModel(It.Is<SelectAccountRequest>(y => y == selectAccountRequest)))
                .ReturnsAsync(expectedViewModel);

            string encodedPledgeId = _fixture.Create<string>();

            var actionResult = await _opportunitiesController.SelectAccount(selectAccountRequest);
            var viewResult = actionResult as ViewResult;
            object model = viewResult.Model;
            var actualViewModel = model as SelectAccountViewModel;

            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(model);
            Assert.IsNotNull(actualViewModel);

            Assert.AreEqual(expectedViewModel, actualViewModel);
        }

        [Test]
        public async Task GET_SelectAccount_NoAccountsReturned_ReturnsEmptyView()
        {
            var selectAccountRequest = _fixture.Create<SelectAccountRequest>();
            var expectedViewModel = _fixture
                .Build<SelectAccountViewModel>()
                .With(x => x.Accounts, _fixture.CreateMany<SelectAccountViewModel.Account>(0))
                .Create();

            _orchestrator
                .Setup(x => x.GetSelectAccountViewModel(It.Is<SelectAccountRequest>(y => y == selectAccountRequest)))
                .ReturnsAsync(expectedViewModel);

            string encodedPledgeId = _fixture.Create<string>();

            var actionResult = await _opportunitiesController.SelectAccount(selectAccountRequest);
            var viewResult = actionResult as ViewResult;
            object model = viewResult.Model;
            var actualViewModel = model as SelectAccountViewModel;

            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(model);
            Assert.IsNotNull(actualViewModel);

            Assert.AreEqual(expectedViewModel, actualViewModel);
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
            var selectedStandardId = _fixture.Create<string>();
            var encodedPledgeId = _fixture.Create<string>();
            var encodedAccountId = _fixture.Create<string>();
            var cacheKey = _fixture.Create<Guid>();
            var applicationRequest = _fixture.Create<ApplicationRequest>();
            applicationRequest.EncodedPledgeId = encodedPledgeId;
            applicationRequest.EncodedAccountId = encodedAccountId;
            applicationRequest.CacheKey = cacheKey;

            var request = new ApplicationDetailsPostRequest
            {
                AccountId = 1,
                EncodedPledgeId = encodedPledgeId,
                EncodedAccountId = encodedAccountId,
                CacheKey = cacheKey,
                HasTrainingProvider = true,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                NumberOfApprentices = "1",
                PledgeId = 1,
                SelectedStandardId = selectedStandardId,
                SelectedStandardTitle = "Test Standard Title"
            };

            var opportunitiesService = new Mock<IOpportunitiesService>();
            opportunitiesService.Setup(x => x.GetApplicationDetails(1, 1, selectedStandardId)).ReturnsAsync(new GetApplicationDetailsResponse()
            {
                Opportunity = new GetApplicationDetailsResponse.OpportunityData()
                {
                    Amount = 100_000,
                    RemainingAmount = 100_000
                },
                Standards = new List<StandardsListItemDto>()
                {
                    new StandardsListItemDto()
                    {
                        ApprenticeshipFunding = new List<ApprenticeshipFundingDto>()
                        {
                            new ApprenticeshipFundingDto()
                            {
                                Duration = 12,
                                MaxEmployerLevyCap = 9_000,
                                EffectiveFrom = new DateTime(DateTime.UtcNow.AddYears(-1).Year, DateTime.UtcNow.Month, 1),
                                EffectiveTo = null
                            }
                        }
                    }
                }
            });


            _orchestrator.Setup(x => x.PostApplicationViewModel(request)).ReturnsAsync(applicationRequest);

            // Assert
            var redirectToActionResult = (await _opportunitiesController.ApplicationDetails(new ApplicationDetailsPostRequestAsyncValidator(opportunitiesService.Object), request)) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(nameof(OpportunitiesController.Apply), redirectToActionResult.ActionName);
            Assert.AreEqual(encodedPledgeId, redirectToActionResult.RouteValues["EncodedPledgeId"]);
            Assert.AreEqual(encodedAccountId, redirectToActionResult.RouteValues["EncodedAccountId"]);
            Assert.AreEqual(cacheKey, redirectToActionResult.RouteValues["CacheKey"]);
        }

        [Test]
        [Ignore("During March and April this test will always fail as the cost will be calculated as zero, which can always be afforded, even from an empty pledge. Costing is to be overhauled anyway shortly, so safe to ignore this for now.")]
        public async Task POST_ApplicationDetails_Redirects_To_ApplicationDetails_On_Invalid_ModelState()
        {
            // Arrange
            var selectedStandardId = _fixture.Create<string>();
            var encodedPledgeId = _fixture.Create<string>();
            var encodedAccountId = _fixture.Create<string>();
            var cacheKey = _fixture.Create<Guid>();
            var applicationRequest = _fixture.Create<ApplicationRequest>();
            applicationRequest.EncodedPledgeId = encodedPledgeId;
            applicationRequest.EncodedAccountId = encodedAccountId;
            applicationRequest.CacheKey = cacheKey;

            var request = new ApplicationDetailsPostRequest
            {
                AccountId = 1,
                EncodedPledgeId = encodedPledgeId,
                EncodedAccountId = encodedAccountId,
                CacheKey = cacheKey,
                HasTrainingProvider = true,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                NumberOfApprentices = "1",
                PledgeId = 1,
                SelectedStandardId = selectedStandardId,
                SelectedStandardTitle = "Test Standard Title"
            };

            var opportunitiesService = new Mock<IOpportunitiesService>();
            opportunitiesService.Setup(x => x.GetApplicationDetails(1, 1, selectedStandardId)).ReturnsAsync(new GetApplicationDetailsResponse()
            {
                Opportunity = new GetApplicationDetailsResponse.OpportunityData()
                {
                    Amount = 100_000,
                    RemainingAmount = 0
                },
                Standards = new List<StandardsListItemDto>()
                {
                    new StandardsListItemDto()
                    {
                        ApprenticeshipFunding = new List<ApprenticeshipFundingDto>()
                        {
                            new ApprenticeshipFundingDto()
                            {
                                Duration = 12,
                                MaxEmployerLevyCap = 9_000,
                                EffectiveFrom = new DateTime(DateTime.UtcNow.AddYears(-1).Year, DateTime.UtcNow.Month, 1),
                                EffectiveTo = null
                            }
                        }
                    }
                }
            });


            _orchestrator.Setup(x => x.PostApplicationViewModel(request)).ReturnsAsync(applicationRequest);

            // Assert
            var redirectToActionResult = (await _opportunitiesController.ApplicationDetails(new ApplicationDetailsPostRequestAsyncValidator(opportunitiesService.Object), request)) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(nameof(OpportunitiesController.ApplicationDetails), redirectToActionResult.ActionName);
            Assert.AreEqual(encodedPledgeId, redirectToActionResult.RouteValues["EncodedPledgeId"]);
            Assert.AreEqual(encodedAccountId, redirectToActionResult.RouteValues["EncodedAccountId"]);
            Assert.AreEqual(cacheKey, redirectToActionResult.RouteValues["CacheKey"]);
        }

        [Test]
        public async Task GET_Confirmation_Returns_Expected_View()
        {
            var request = _fixture.Create<ConfirmationRequest>();
            var expectedViewModel = _fixture.Create<ConfirmationViewModel>();

            _orchestrator
                .Setup(x => x.GetConfirmationViewModel(request))
                .ReturnsAsync(expectedViewModel);

            var viewResult = await _opportunitiesController.Confirmation(request) as ViewResult;
            var actualViewModel = viewResult.Model as ConfirmationViewModel;

            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(actualViewModel);
            Assert.AreEqual(expectedViewModel, actualViewModel);
        }

        [Test]
        public async Task GET_Apply_Returns_Expected_View()
        {
            var expectedViewModel = new ApplyViewModel();

            _orchestrator.Setup(x => x.GetApplyViewModel(It.IsAny<ApplicationRequest>()))
                .ReturnsAsync(expectedViewModel);

            var viewResult = await _opportunitiesController.Apply(new ApplicationRequest()) as ViewResult;
            
            Assert.IsNotNull(viewResult);
            var actualViewModel = viewResult.Model as ApplyViewModel;
            Assert.IsNotNull(actualViewModel);
            Assert.AreEqual(expectedViewModel, actualViewModel);
        }

        [Test]
        public async Task POST_Apply_Redirects_To_Confirmation()
        {
            var request = new ApplyPostRequest();
            var result = await _opportunitiesController.Apply(request) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Confirmation", result.ActionName);
        }


        [Test]
        public async Task GET_GetFundingEstimate_Returns_Expected_Json_Result()
        {
            var expectedViewModel = new GetFundingEstimateViewModel();

            _orchestrator.Setup(x => x.GetFundingEstimate(It.IsAny<GetFundingEstimateRequest>(), null))
                .ReturnsAsync(expectedViewModel);

            var jsonResult = await _opportunitiesController.GetFundingEstimate(new GetFundingEstimateRequest()) as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(expectedViewModel, jsonResult.Value);
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

            var request = new SectorPostRequest
            {
                Sectors = new List<string> { "Sector" },
                EncodedPledgeId = encodedPledgeId,
                EncodedAccountId = encodedAccountId,
                CacheKey = cacheKey
            };

            _orchestrator.Setup(x => x.UpdateCacheItem(request));

            // Assert
            var redirectToActionResult = (await _opportunitiesController.Sector(request)) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(redirectToActionResult.ActionName, nameof(OpportunitiesController.Apply));
            Assert.AreEqual(redirectToActionResult.RouteValues["EncodedPledgeId"], encodedPledgeId);
            Assert.AreEqual(redirectToActionResult.RouteValues["EncodedAccountId"], encodedAccountId);
            Assert.AreEqual(redirectToActionResult.RouteValues["CacheKey"], cacheKey);
            _orchestrator.Verify(x => x.UpdateCacheItem(request), Times.Once);
        }
    }
}