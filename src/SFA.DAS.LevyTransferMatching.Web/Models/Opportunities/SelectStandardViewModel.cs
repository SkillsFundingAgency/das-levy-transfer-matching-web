using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class SelectStandardViewModel
    {
        public IEnumerable<StandardsListItemViewModel> Standards { get; set; }
        public string SelectedStandardId { get; set; }
        public string SelectedStandardTitle { get; set; }
    }
}