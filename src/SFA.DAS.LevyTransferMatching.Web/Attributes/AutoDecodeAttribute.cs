using System;
using SFA.DAS.Encoding;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes
{
    public class AutoDecodeAttribute : Attribute
    {
        public string Source { get; set; }
        public EncodingType EncodingType { get; set; }

        public AutoDecodeAttribute(string source, EncodingType encodingType)
        {
            Source = source;
            EncodingType = encodingType;
        }
    }
}
