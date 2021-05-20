using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers
{
    [TestFixture]
    public class PledgesControllerTests
    {
        private Fixture _fixture;
        private PledgesController _pledgesController;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _pledgesController = new PledgesController();
        }

        [Test]
        public void GET_Index_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            string encodedAccountId = _fixture.Create<string>();

            // Act
            ViewResult viewResult = _pledgesController.Index(encodedAccountId) as ViewResult;
            IndexViewModel indexViewModel = viewResult?.Model as IndexViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(indexViewModel);
            Assert.AreEqual(encodedAccountId, indexViewModel.EncodedAccountId);
        }

        [Test]
        public void GET_Create_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            string encodedAccountId = _fixture.Create<string>();

            // Act
            ViewResult viewResult = _pledgesController.Create(encodedAccountId) as ViewResult;
            CreateViewModel createViewModel = viewResult?.Model as CreateViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(createViewModel);
            Assert.AreEqual(encodedAccountId, createViewModel.EncodedAccountId);
        }

        [Test]
        public void GET_PledgeAmount_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            string encodedAccountId = _fixture.Create<string>();

            // Act
            ViewResult viewResult = _pledgesController.PledgeAmount(encodedAccountId) as ViewResult;
            PledgeAmountViewModel pledgeAmountViewModel = viewResult?.Model as PledgeAmountViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(pledgeAmountViewModel);
            Assert.AreEqual(encodedAccountId, pledgeAmountViewModel.EncodedAccountId);
        }

        [Test]
        public void POST_PledgeAmount_Returns_Expected_Redirect()
        {
            // Arrange
            PledgeAmountViewModel pledgeAmountViewModel = _fixture.Create<PledgeAmountViewModel>();

            // Act
            RedirectToActionResult actionResult = _pledgesController.PledgeAmount(pledgeAmountViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Create", actionResult.ActionName);
            Assert.AreEqual(pledgeAmountViewModel.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
        }

        [Test]
        public void POST_PledgeAmount_Returns_Expected_View_With_Expected_ViewModel_When_Model_Invalid()
        {
            // Arrange
            PledgeAmountViewModel pledgeAmountViewModel = _fixture.Create<PledgeAmountViewModel>();
            _pledgesController.ModelState.AddModelError("TestError", "TestError");

            // Act
            ViewResult viewResult = _pledgesController.PledgeAmount(pledgeAmountViewModel) as ViewResult;
            PledgeAmountViewModel resultViewModel = viewResult?.Model as PledgeAmountViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(resultViewModel);
            Assert.AreEqual(pledgeAmountViewModel.EncodedAccountId, resultViewModel.EncodedAccountId);
            Assert.AreEqual(pledgeAmountViewModel.PledgeAmount, resultViewModel.PledgeAmount);
            Assert.AreEqual(pledgeAmountViewModel.IsNamePublic, resultViewModel.IsNamePublic);
        }
    }
}