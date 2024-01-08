using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Shared
{
    public interface ICookieBannerViewModel
    {
        string CookieConsentUrl { get; }
        string CookieDetailsUrl { get; }
    }

    public class CookieBannerViewModel : ICookieBannerViewModel
    {
        public string CookieDetailsUrl { get; set; }
        public string CookieConsentUrl { get; set; }
    }
}
