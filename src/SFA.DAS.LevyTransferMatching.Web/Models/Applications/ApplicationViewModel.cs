using System;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Applications
{
    public class ApplicationViewModel : ApplicationRequest
    {
        public IEnumerable<string> Locations { get; set; }
        public bool IsNamePublic { get; set; }
        public string PledgeEmployerAccountName { get; set; }
        public ApplicationStatus Status { get; set; }
        public string JobRole { get; set; }
        public int Level { get; set; }
        public int NumberOfApprentices { get; set; }
        public DateTime StartBy { get; set; }
        public OpportunitySummaryViewModel OpportunitySummaryViewModel { get; set; }
        public string EncodedOpportunityId { get; set; }
        public string Title
        {
            get
            {
                return Status switch
                {
                    ApplicationStatus.Accepted =>
                        $"Use transfer funds from {(IsNamePublic ? EmployerAccountName : "opportunity")} {EncodedOpportunityId}",
                    _ =>
        public string Title => $"Your {(IsNamePublic ? EmployerAccountName : "opportunity")} ({EncodedOpportunityId}) application details";
                };
            }
        }

        public bool CanAcceptFunding { get; set; }
        public bool CanUseTransferFunds { get; set; }
        public ApprovalAction? SelectedAction { get; set; }
        public bool TruthfulInformation { get; set; }
        public bool ComplyWithRules { get; set; }
        public bool HasAcceptedTermsAndConditions => TruthfulInformation && ComplyWithRules && SelectedAction == ApprovalAction.Accept;
        public string EstimatedTotalCost { get; set; }
        public bool RenderCanUseTransferFundsStartButton { get; set; }
        public enum ApprovalAction
        {
            Accept,
            Decline,
        }
    }
}