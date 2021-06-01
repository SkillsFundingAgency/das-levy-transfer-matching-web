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
        public void GET_Amount_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            string encodedAccountId = _fixture.Create<string>();

            // Act
            ViewResult viewResult = _pledgesController.Amount(encodedAccountId) as ViewResult;
            AmountViewModel amountViewModel = viewResult?.Model as AmountViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(amountViewModel);
            Assert.AreEqual(encodedAccountId, amountViewModel.EncodedAccountId);
        }

        [Test]
        public void POST_Amount_Returns_Expected_Redirect()
        {
            // Arrange
            AmountPostModel amountPostModel = _fixture.Create<AmountPostModel>();

            // Act
            RedirectToActionResult actionResult = _pledgesController.Amount(amountPostModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Index", actionResult.ActionName);
            Assert.AreEqual(amountPostModel.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
        }
    }
}