using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using AutoFixture;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Services;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Services
{
    public class CsvHelperServiceTests
    {
        private Fixture _fixture;
        private CsvHelperService _csvService;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _csvService = new CsvHelperService();
        }

        [Test]
        public void GenerateCsvFileFromModel_When_Given_A_Model_Returns_A_Byte_Array()
        {
            var model = _fixture.Create<PledgeApplicationsDownloadModel>();
            foreach (var pledgeApplicationDownloadModel in model.Applications)
            {
                pledgeApplicationDownloadModel.DynamicLocations = null;
            }
            var actual = _csvService.GenerateCsvFileFromModel(model);

            Assert.That(actual.Length > 0, Is.True);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(3, true)]
        public void AddLocationColumns_WhenGivenData_SetsTheExactAmountOfColumnsAcrossAllApplications(int totalLocationColumnsRequired, bool expectedOutcome)
        {
            var model = _fixture.Create<PledgeApplicationDownloadModel>();
            dynamic locationObject = new ExpandoObject();
            locationObject.Name = "Sheffield";
            model.DynamicLocations = new List<dynamic>
            {
                locationObject
            };

            dynamic returnModel = new ExpandoObject();
            CsvHelperService.AddLocationColumns(model, returnModel, totalLocationColumnsRequired);

            var expandoDict = returnModel as IDictionary<string, object>;
            Assert.That(expandoDict.ContainsKey($"Location{totalLocationColumnsRequired}"), Is.EqualTo(expectedOutcome));
        }
    }
}
