using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto
{
    public class OpportunityDto
    {
        public int Id { get; set; }
        public string DasAccountName { get; set; }
        public IEnumerable<string> JobRoles { get; set; }
        public IEnumerable<string> Levels { get; set; }
        public IEnumerable<string> Sectors { get; set; }
        public IEnumerable<string> Locations { get; set; }
        public int Amount { get; set; }
        public bool IsNamePublic { get; set; }
        public int RemainingAmount { get; set; }
    }
}