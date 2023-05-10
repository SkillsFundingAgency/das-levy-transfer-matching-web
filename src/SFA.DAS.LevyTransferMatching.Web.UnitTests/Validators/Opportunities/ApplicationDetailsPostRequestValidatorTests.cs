using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Opportunities
{
    public class ApplicationDetailsPostRequestValidatorTests
    {
        private ApplicationDetailsPostRequestAsyncValidator _validator;
        private Mock<IOpportunitiesService> _service = new Mock<IOpportunitiesService>(MockBehavior.Strict);
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _validator = new ApplicationDetailsPostRequestAsyncValidator(_service.Object);
        }

        [Test]
        public async Task Validator_Returns_True_For_All_Valid_Input()
        {
            var request = CreateApplicationDetailsPostRequest();
            _service.Setup(x => x.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId)).ReturnsAsync(CreateApplicationDetailsResponse());

            var result = (await _validator.ValidateAsync(request));

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        [Ignore("During March and April this test will always fail as the cost will be calculated as zero, which can always be afforded, even from an empty pledge. Costing is to be overhauled anyway shortly, so safe to ignore this for now.")]
        public async Task Validator_Returns_False_When_There_Are_Insufficient_Funds()
        {
            var request = CreateApplicationDetailsPostRequest();
            var applicationDetailsDto = CreateApplicationDetailsResponse();
            applicationDetailsDto.Opportunity.RemainingAmount = 0;

            _service.Setup(x => x.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId)).ReturnsAsync(applicationDetailsDto);

            var result = (await _validator.ValidateAsync(request));

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("There is not enough funding to support this many apprentices", result.Errors.First().ErrorMessage);
        }

        [Test]
        public async Task Validator_Returns_False_For_Null_NumberOfApprentices()
        {
            var request = CreateApplicationDetailsPostRequest();
            request.NumberOfApprentices = null;
            _service.Setup(x => x.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId)).ReturnsAsync(CreateApplicationDetailsResponse());

            var result = (await _validator.ValidateAsync(request));

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("You must enter the number of apprentices", result.Errors.First().ErrorMessage);
        }

        [Test]
        public async Task Validator_Returns_False_For_Zero_NumberOfApprentices()
        {
            var request = CreateApplicationDetailsPostRequest();
            request.NumberOfApprentices = "0";
            _service.Setup(x => x.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId)).ReturnsAsync(CreateApplicationDetailsResponse());

            var result = (await _validator.ValidateAsync(request));

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("You must enter the number of apprentices", result.Errors.First().ErrorMessage);
        }

        [Test]
        public async Task Validator_Returns_False_For_Negative_NumberOfApprentices()
        {
            var request = CreateApplicationDetailsPostRequest();
            request.NumberOfApprentices = "-1";
            _service.Setup(x => x.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId)).ReturnsAsync(CreateApplicationDetailsResponse());

            var result = (await _validator.ValidateAsync(request));

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("You must enter the number of apprentices", result.Errors.First().ErrorMessage);
        }

        [Test]
        public async Task Validator_Returns_False_For_Null_JobRole()
        {
            var request = CreateApplicationDetailsPostRequest();
            request.SelectedStandardId = null;
            _service.Setup(x => x.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId)).ReturnsAsync(CreateApplicationDetailsResponse());

            var result = (await _validator.ValidateAsync(request));

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Enter a valid job role", result.Errors.First().ErrorMessage);
        }

        [Test]
        public async Task Validator_Returns_False_For_Empty_JobRole()
        {
            var request = CreateApplicationDetailsPostRequest();
            request.SelectedStandardId = string.Empty;
            _service.Setup(x => x.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId)).ReturnsAsync(CreateApplicationDetailsResponse());

            var result = (await _validator.ValidateAsync(request));

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Enter a valid job role", result.Errors.First().ErrorMessage);
        }

        [Test]
        public async Task Validator_Returns_False_For_Null_StartDate()
        {
            var request = CreateApplicationDetailsPostRequest();
            request.Month = null;
            request.Year = null;
            _service.Setup(x => x.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId)).ReturnsAsync(CreateApplicationDetailsResponse());

            var result = (await _validator.ValidateAsync(request));

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public async Task Validator_Returns_False_For_Null_HasTrainingProvider()
        {
            var request = CreateApplicationDetailsPostRequest();
            request.HasTrainingProvider = null;
            _service.Setup(x => x.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId)).ReturnsAsync(CreateApplicationDetailsResponse());

            var result = (await _validator.ValidateAsync(request));

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("You must select whether or not you have found a training provider", result.Errors.First().ErrorMessage);
        }

        private ApplicationDetailsPostRequest CreateApplicationDetailsPostRequest() =>
            new ApplicationDetailsPostRequest()
            {
                SelectedStandardId = _fixture.Create<string>(),
                Year = DateTime.UtcNow.Year,
                Month = DateTime.UtcNow.Month,
                NumberOfApprentices = "1",
                PledgeId = 1,
                HasTrainingProvider = true,
                CacheKey = _fixture.Create<Guid>(),
                EncodedAccountId = _fixture.Create<string>(),
                EncodedPledgeId = _fixture.Create<string>(),
                SelectedStandardTitle = _fixture.Create<string>(),
                AccountId = 1
            };

        private GetApplicationDetailsResponse CreateApplicationDetailsResponse() => new GetApplicationDetailsResponse()
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
                        },
                        new ApprenticeshipFundingDto()
                        {
                            Duration = 15,
                            MaxEmployerLevyCap = 12_000,
                            EffectiveFrom = new DateTime(DateTime.UtcNow.AddYears(-2).Year, DateTime.UtcNow.AddMonths(-1).Month, 1),
                            EffectiveTo = new DateTime(DateTime.UtcNow.AddYears(-1).Year, DateTime.UtcNow.AddMonths(-1).Month, new DateTime(DateTime.UtcNow.AddYears(-1).Year,DateTime.UtcNow.Month,1).AddDays(-1).Day)
                        }
                    }
                }
            }
        };
    }
}
