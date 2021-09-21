namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LocationSelectPostRequest : LocationSelectRequest
    {
        public SelectValidLocationGroup[] SelectValidLocationGroups { get; set; }

        public class SelectValidLocationGroup
        {
            public int Index { get; set; }
            public string SelectedValue { get; set; }
            public ValidLocationItem[] ValidLocationItems { get; set; }

            public class ValidLocationItem
            {
                public string Value { get; set; }
                public bool Selected { get; set; }
            }
        }
    }
}