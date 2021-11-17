namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationApprovalOptionsViewModel : ApplicationApprovalOptionsPostRequest
    {
        public string EmployerAccountName { get; set; }
        public bool IsApplicationPending { get; set; }
    }
}
