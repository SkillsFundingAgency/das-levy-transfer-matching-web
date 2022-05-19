using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions
{
    public class ApprenticeshipFundingDtoExtensionsTests
    {
        [Test]
        public void GetEffectiveFundingLine_Returns_Latest_Line()
        {
            var sut = new List<ApprenticeshipFundingDto>()
            {
                new ApprenticeshipFundingDto()
                {
                    Duration = 12,
                    MaxEmployerLevyCap = 9_000,
                    EffectiveFrom = new DateTime(2021, 1, 1),
                    EffectiveTo = null
                },
                new ApprenticeshipFundingDto()
                {
                    Duration = 15,
                    MaxEmployerLevyCap = 12_000,
                    EffectiveFrom = new DateTime(2020, 12, 1),
                    EffectiveTo = new DateTime(2021,12, 30)
                }
            }.AsEnumerable();

            var result = sut.GetEffectiveFundingLine(DateTime.UtcNow.Date);

            Assert.IsNotNull(result);
            Assert.AreEqual(12, result.Duration);
            Assert.AreEqual(new DateTime(2021, 1, 1), result.EffectiveFrom);
            Assert.IsNull(result.EffectiveTo);
            Assert.AreEqual(9_000, result.MaxEmployerLevyCap);
        }

        [Test]
        public void GetEffectiveFundingLine_Returns_Historical_Line()
        {
            var now = new DateTime(2021,08, 30);

            var sut = new List<ApprenticeshipFundingDto>()
            {
                new ApprenticeshipFundingDto()
                {
                    Duration = 12,
                    MaxEmployerLevyCap = 9_000,
                    EffectiveFrom = new DateTime(now.Year, now.AddMonths(1).Month, 1),
                    EffectiveTo = null
                },
                new ApprenticeshipFundingDto()
                {
                    Duration = 15,
                    MaxEmployerLevyCap = 12_000,
                    EffectiveFrom = new DateTime(now.AddYears(-2).Year, now.AddMonths(-1).Month, 1),
                    EffectiveTo = new DateTime(now.Year, now.Month, 30)
                }
            }.AsEnumerable();

            var result = sut.GetEffectiveFundingLine(now.Date);

            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.Duration);
            Assert.AreEqual(new DateTime(now.AddYears(-2).Year, now.AddMonths(-1).Month, 1), result.EffectiveFrom);
            Assert.AreEqual(new DateTime(now.Year, now.Month, 30), result.EffectiveTo);
            Assert.AreEqual(12_000, result.MaxEmployerLevyCap);
        }

        [Test]
        public void CalcFundingForDate_Calculates_Correctly_For_1_Apprentice_15_Months()
        {
            var sut = new ApprenticeshipFundingDto()
            {
                Duration = 15,
                MaxEmployerLevyCap = 11_000,
                EffectiveFrom = new DateTime(2021, 12, 1),
                EffectiveTo = null
            };

            var result = sut.CalcFundingForDate(1, new DateTime(2021, 9, 1));

            Assert.AreEqual(3500, result);
        }

        [Test]
        public void CalcFundingForDate_Calculates_Correctly_For_2_Apprentices_24_Months()
        {
            var sut = new ApprenticeshipFundingDto()
            {
                Duration = 24,
                MaxEmployerLevyCap = 20_000,
                EffectiveFrom = new DateTime(2021, 12, 1),
                EffectiveTo = null
            };

            var result = sut.CalcFundingForDate(2, new DateTime(2021, 10, 1));

            Assert.AreEqual(6700, result);
        }

        [Test]
        public void CalcFundingForDate_Calculates_Correctly_For_1_Apprentice_12_Months()
        {
            var sut = new ApprenticeshipFundingDto()
            {
                Duration = 12,
                MaxEmployerLevyCap = 12_000,
                EffectiveFrom = new DateTime(2021, 12, 1),
                EffectiveTo = null
            };

            var result = sut.CalcFundingForDate(1, new DateTime(2021, 2, 1));

            Assert.AreEqual(800, result);
        }

        [TestCase(1, 15000, 15000)]
        [TestCase(2, 15000, 30000)]
        public void Calculate_Estimated_Total_Cost_Calculates_Correctly_When_Given_Inputs(int numberOfApprentices, int maxLevyCap, int expected)
        {
            var dto = new ApprenticeshipFundingDto
            {
                Duration = 24,
                EffectiveFrom = new DateTime(2021, 12, 1),
                EffectiveTo = null,
                MaxEmployerLevyCap = maxLevyCap
            };

            var result = dto.CalculateEstimatedTotalCost(numberOfApprentices);

            Assert.AreEqual(expected, result);
        }
    }
}
