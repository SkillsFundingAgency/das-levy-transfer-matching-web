using System;
using System.Collections.Generic;
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

            Assert.IsTrue(actual.Length > 0);
        }
    }
}
