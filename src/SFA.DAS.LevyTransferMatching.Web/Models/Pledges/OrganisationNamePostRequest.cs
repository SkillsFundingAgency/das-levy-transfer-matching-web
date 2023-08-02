namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class OrganisationNamePostRequest : BasePledgesRequest
    {
        public bool? IsNamePublic { get; set; }

        public string DasAccountName { get; set; }

    }
}
