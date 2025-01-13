using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions;

public class ApprenticeshipFundingDtoExtensionsTests
{
    [Test]
    public void GetEffectiveFundingLine_Returns_Latest_Line()
    {
        var sut = new List<ApprenticeshipFundingDto>
        {
            new()
            {
                Duration = 12,
                MaxEmployerLevyCap = 9_000,
                EffectiveFrom = new DateTime(2021, 1, 1),
                EffectiveTo = null
            },
            new()
            {
                Duration = 15,
                MaxEmployerLevyCap = 12_000,
                EffectiveFrom = new DateTime(2020, 12, 1),
                EffectiveTo = new DateTime(2021, 12, 30)
            }
        }.AsEnumerable();

        var result = sut.GetEffectiveFundingLine(DateTime.UtcNow.Date);

        result.Should().NotBeNull();
        result.Duration.Should().Be(12);
        result.EffectiveFrom.Should().Be(new DateTime(2021, 1, 1));
        result.EffectiveTo.Should().BeNull();
        result.MaxEmployerLevyCap.Should().Be(9_000);
    }

    [Test]
    public void GetEffectiveFundingLine_Returns_Historical_Line()
    {
        var now = new DateTime(2021, 08, 30);

        var sut = new List<ApprenticeshipFundingDto>
        {
            new()
            {
                Duration = 12,
                MaxEmployerLevyCap = 9_000,
                EffectiveFrom = new DateTime(now.Year, now.AddMonths(1).Month, 1),
                EffectiveTo = null
            },
            new()
            {
                Duration = 15,
                MaxEmployerLevyCap = 12_000,
                EffectiveFrom = new DateTime(now.AddYears(-2).Year, now.AddMonths(-1).Month, 1),
                EffectiveTo = new DateTime(now.Year, now.Month, 30)
            }
        }.AsEnumerable();

        var result = sut.GetEffectiveFundingLine(now.Date);

        result.Should().NotBeNull();
        result.Duration.Should().Be(15);
        result.EffectiveFrom.Should().Be(new DateTime(now.AddYears(-2).Year, now.AddMonths(-1).Month, 1));
        result.EffectiveTo.Should().Be(new DateTime(now.Year, now.Month, 30));
        result.MaxEmployerLevyCap.Should().Be(12_000);
    }

    [TestCase(48000, 24, 0)]
    [TestCase(48000, 24, 1)]
    [TestCase(18000, 18, 1)]
    [TestCase(18000, 12, 1)]
    [TestCase(18000, 12, 2)]
    public void CalculateOneYearCost_Produces_Expected_Result(int fundingCap, int duration, int numberOfApprentices)
    {
        var sut = new ApprenticeshipFundingDto
        {
            Duration = duration,
            MaxEmployerLevyCap = fundingCap,
            EffectiveFrom = new DateTime(2021, 12, 1),
            EffectiveTo = null
        };

        var expectedResult = duration <= 12
            ? fundingCap * numberOfApprentices
            : fundingCap * numberOfApprentices * 0.8m / duration * 12;

        var result = sut.CalculateOneYearCost(numberOfApprentices);

        result.Should().Be((int)expectedResult);
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

        result.Should().Be(expected);
    }
}