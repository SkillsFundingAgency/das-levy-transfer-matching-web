using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.UnitTests.Services
{
    public class WhenConvertingFromApiResponseToEmployerUserAccounts
    {
        [Test, AutoData]
        public void Then_The_Values_Are_Mapped(GetUserAccountsResponse source)
        {
            source.IsSuspended = true;
        
            var actual = (EmployerUserAccounts) source;

            actual.EmployerAccounts.Should().BeEquivalentTo(source.UserAccounts);
            actual.IsSuspended.Should().Be(source.IsSuspended);
        }

        [Test]
        public void Then_If_Null_Then_Empty_Returned()
        {
            var actual = (EmployerUserAccounts) (GetUserAccountsResponse)null;

            actual.EmployerAccounts.Should().BeEmpty();
            actual.IsSuspended.Should().BeFalse();
        }
    }
}