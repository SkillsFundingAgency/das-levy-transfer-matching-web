﻿using SFA.DAS.LevyTransferMatching.Domain.Types;
using System;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetPledgesResponse
    {
        public IEnumerable<Pledge> Pledges { get; set; }
        public decimal StartingTransferAllowance { get; set; }
        public IEnumerable<Application> AcceptedAndApprovedApplications{ get; set; }
        public class Pledge
        {
            public int Id { get; set; }
            public int Amount { get; set; }
            public int RemainingAmount { get; set; }
            public int ApplicationCount { get; set; }
            public PledgeStatus Status { get; set; }
        }
        public class Application
        {
            public int Id { get; set; }
            public string DasAccountName { get; set; }
            public int PledgeId { get; set; }
            public int StandardDuration { get; set; }
            public DateTime StartDate { get; set; }
            public int Amount { get; set; }
            public int TotalAmount { get; set; }
            public int CurrentFinancialYearAmount { get; set; }
            public bool HasTrainingProvider { get; set; }
            public DateTime CreatedOn { get; set; }
            public bool IsNamePublic { get; set; }
            public string Status { get; set; }
            public bool IsLocationMatch { get; set; }
            public bool IsSectorMatch { get; set; }
            public bool IsJobRoleMatch { get; set; }
            public bool IsLevelMatch { get; set; }
            public int MatchPercentage { get; set; }
            public string EmployerAccountName { get; set; }
            public IEnumerable<ApplicationLocation> Locations { get; set; }
            public IEnumerable<string> Sectors { get; set; }
            public int NumberOfApprentices { get; set; }
            public string Details { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public IEnumerable<string> EmailAddresses { get; set; }
            public string BusinessWebsite { get; set; }
            public string JobRole { get; set; }
            public int PledgeRemainingAmount { get; set; }
            public int StandardMaxFunding { get; set; }
            public int StandardLevel { get; set; }
            public List<LocationDataItem> PledgeLocations { get; set; }
            public string AdditionalLocations { get; set; }
            public string SpecificLocation { get; set; }

        }

        public class ApplicationLocation
        {
            public int Id { get; set; }
            public int PledgeLocationId { get; set; }
        }

        public class LocationDataItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double[] GeoPoint { get; set; }
        }
    }
}
