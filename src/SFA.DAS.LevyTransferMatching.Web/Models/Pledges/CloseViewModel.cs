
namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class CloseViewModel : ClosePostRequest
    {
        public bool UserCanClosePledge { get; set; }

        public bool PledgeClosed { get; set; }
    }
}