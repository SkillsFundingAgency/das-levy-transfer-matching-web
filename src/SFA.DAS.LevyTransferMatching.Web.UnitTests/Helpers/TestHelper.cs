using KellermanSoftware.CompareNetObjects;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Helpers;

public class TestHelper
{
    public static T Clone<T>(T source)
    {
        var serialized = JsonConvert.SerializeObject(source);
        return JsonConvert.DeserializeObject<T>(serialized);
    }

    public static bool EnumerablesAreEqual(IEnumerable<object> expected, IEnumerable<object> actual)
    {
        return new CompareLogic(new ComparisonConfig { IgnoreObjectTypes = true })
            .Compare(expected, actual).AreEqual;
    }

    public static T GetRandomFlagsValue<T>() where T : Enum
    {
        var enumType = typeof(T);

        var enumValues = enumType.GetEnumValues();
        var lastEnumValue = enumValues.GetValue(enumValues.Length - 1);
        var lastIntValue = (int)Convert.ChangeType(lastEnumValue, TypeCode.Int32);

        var max = (lastIntValue * 2) - 1;

        var result = new Random().Next(0, max);

        return (T)Enum.ToObject(enumType, result);
    }
}