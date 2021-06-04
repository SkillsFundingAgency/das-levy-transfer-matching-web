using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgesService
{
    public interface IPledgesService
    {
        public Task PostPledge(PledgeDto pledgeDto);
    }
}
