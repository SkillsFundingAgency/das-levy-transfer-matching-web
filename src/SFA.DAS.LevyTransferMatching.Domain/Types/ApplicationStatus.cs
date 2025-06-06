﻿namespace SFA.DAS.LevyTransferMatching.Domain.Types;

public enum ApplicationStatus : byte
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Accepted = 3,
    FundsUsed = 4,
    Declined = 5,
    Withdrawn = 6,
    WithdrawnAfterAcceptance = 7,
    FundsExpired = 8
}
