using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.LevyTransferMatching.Domain.EmployerAccounts
{

    public class AccountUser
    {
        public Guid UserRef { get; set; }
        public HashSet<UserRole> Roles { get; set; }
    }
}
