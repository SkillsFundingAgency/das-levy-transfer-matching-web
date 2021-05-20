using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers
{
    [TestFixture]
    public class PledgesControllerTests
    {
        private PledgesController pledgesController;

        [SetUp]
        public void Setup()
        {
            pledgesController = new PledgesController();
        }

        [Test]
        public void GET_Index_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            string encodedAccountId = "abc";
            ViewResult viewResult = null;
            IndexViewModel indexViewModel = null;

            // Act
            viewResult = this.pledgesController.Index(encodedAccountId) as ViewResult;
            indexViewModel = viewResult?.Model as IndexViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(indexViewModel);
            Assert.AreEqual(encodedAccountId, indexViewModel.EncodedAccountId);
        }

        [Test]
        public void GET_Create_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            string encodedAccountId = "abc";
            ViewResult viewResult = null;
            CreateViewModel createViewModel = null;

            // Act
            viewResult = this.pledgesController.Create(encodedAccountId) as ViewResult;
            createViewModel = viewResult?.Model as CreateViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(createViewModel);
            Assert.AreEqual(encodedAccountId, createViewModel.EncodedAccountId);
        }
    }
}