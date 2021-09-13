using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LocationSelectPostRequest : LocationSelectRequest
    {
        public LocationSelectionGroup[] LocationSelectionGroups { get; set; }

        public class LocationSelectionGroup
        {
            public int Index
            {
                get;
                set;
            }

            public string SelectedValue { get; set; }

            public LocationSelectionItem[] LocationSelectionItems { get; set; }

            public class LocationSelectionItem
            {
                public string Value { get; set; }
                public bool Selected { get; set; }
            }
        }
    }
}