using AutoFixture;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Models.Opportunities
{
    public class ApplyViewModelTests
    {
        private const string IncompleteString = "-";
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void ApplyViewModel_IsApprenticeshipTrainingSectionComplete_Returns_True_When_All_Fields_Entered()
        {
            var model = _fixture.Create<ApplyViewModel>();

            Assert.IsTrue(model.IsApprenticeshipTrainingSectionComplete);
        }

        [Test]
        public void ApplyViewModel_IsApprenticeshipTrainingSectionComplete_Returns_False_When_One_Field_Not_Entered()
        {
            var model = _fixture.Create<ApplyViewModel>();
            model.HaveTrainingProvider = IncompleteString;

            Assert.IsFalse(model.IsApprenticeshipTrainingSectionComplete);
        }

        [Test]
        public void ApplyViewModel_IsBusinessDetailsSectionComplete_Returns_True_When_All_Fields_Entered()
        {
            var model = _fixture.Create<ApplyViewModel>();

            Assert.IsTrue(model.IsBusinessDetailsSectionComplete);
        }

        [Test]
        public void ApplyViewModel_IsBusinessDetailsSectionComplete_Returns_False_When_One_Field_Not_Entered()
        {
            var model = _fixture.Create<ApplyViewModel>();
            model.Sectors = IncompleteString;

            Assert.IsFalse(model.IsBusinessDetailsSectionComplete);
        }

        [Test]
        public void ApplyViewModel_IsContactDetailsSectionComplete_Returns_True_When_All_Fields_Entered()
        {
            var model = _fixture.Create<ApplyViewModel>();

            Assert.IsTrue(model.IsContactDetailsSectionComplete);
        }

        [Test]
        public void ApplyViewModel_IsContactDetailsSectionComplete_Returns_False_When_One_Field_Not_Entered()
        {
            var model = _fixture.Create<ApplyViewModel>();
            model.ContactName = IncompleteString;

            Assert.IsFalse(model.IsContactDetailsSectionComplete);
        }

        [Test]
        public void ApplyViewModel_IsComplete_Returns_True_When_All_Other_Sections_Complete()
        {
            var model = _fixture.Create<ApplyViewModel>();

            Assert.IsTrue(model.IsComplete);
        }

        [Test]
        public void ApplyViewModel_IsComplete_Returns_False_When_Training_Sections_Incomplete()
        {
            var model = _fixture.Create<ApplyViewModel>();
            model.JobRole = IncompleteString;

            Assert.IsFalse(model.IsComplete);
        }

        [Test]
        public void ApplyViewModel_IsComplete_Returns_False_When_BusinessDetails_Sections_Incomplete()
        {
            var model = _fixture.Create<ApplyViewModel>();
            model.Sectors = IncompleteString;

            Assert.IsFalse(model.IsComplete);
        }

        [Test]
        public void ApplyViewModel_IsComplete_Returns_False_When_ContactDetails_Sections_Incomplete()
        {
            var model = _fixture.Create<ApplyViewModel>();
            model.ContactName = IncompleteString;

            Assert.IsFalse(model.IsComplete);
        }
    }
}
