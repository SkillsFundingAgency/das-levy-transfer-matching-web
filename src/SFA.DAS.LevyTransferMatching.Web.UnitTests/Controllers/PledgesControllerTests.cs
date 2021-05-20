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
    }
}