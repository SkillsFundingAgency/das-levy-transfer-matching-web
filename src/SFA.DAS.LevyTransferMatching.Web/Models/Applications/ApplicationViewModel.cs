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
                        $"Use transfer funds from {(IsNamePublic ? PledgeEmployerAccountName : "opportunity")} {EncodedOpportunityId}",
                    _ =>
                     $"Your {(IsNamePublic ? PledgeEmployerAccountName : "opportunity")} ({EncodedOpportunityId}) application details"
                };
            }
        }

        public bool CanAcceptFunding { get; set; }
        public bool CanUseTransferFunds { get; set; }
        public ApprovalAction? SelectedAction { get; set; }
        public bool TruthfulInformation { get; set; }
        public bool ComplyWithRules { get; set; }
        public bool HasAcceptedTermsAndConditions => SelectedAction != null && TruthfulInformation && ComplyWithRules;
        public bool AllowTransferRequestAutoApproval { get; set; }
        public bool IsDeclineConfirmed { get; set; }

        public string EstimatedTotalCost { get; set; }
        public bool RenderCanUseTransferFundsStartButton { get; set; }
        public bool CanWithdraw { get; set; }
        public bool IsWithdrawalConfirmed { get; set; }
        public enum ApprovalAction
        {
            Accept,
            Decline,
            Withdraw,
            None
        }

        public bool DisplayCurrentFundsBalance { get; set; }
        public string Amount { get; set; }
        public string AmountUsed { get; set; }
        public string AmountRemaining { get; set; }
        public int NumberOfApprenticesRemaining { get; set; }
    }
}