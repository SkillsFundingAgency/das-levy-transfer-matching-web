namespace SFA.DAS.LevyTransferMatching.Domain.Types
{
    public enum ApplicationStatus : byte
    {
        Pending = 0,
        Approved = 1,
        Accepted = 3,
        FundsUsed = 4,
        Withdrawn = 6
    }
}
