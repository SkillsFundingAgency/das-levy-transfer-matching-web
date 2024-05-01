using SFA.DAS.Encoding;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class AutoDecodeAttribute : Attribute
{
    public string Source { get; }
    public EncodingType EncodingType { get; }

    public AutoDecodeAttribute(string source, EncodingType encodingType)
    {
        Source = source;
        EncodingType = encodingType;
    }
}