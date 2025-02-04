using SFA.DAS.Encoding;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class AutoDecodeAttribute(string source, EncodingType encodingType) : Attribute
{
    public string Source { get; } = source;
    public EncodingType EncodingType { get; } = encodingType;
}