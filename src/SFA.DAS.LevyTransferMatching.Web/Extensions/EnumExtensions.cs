using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var displayAttribute = GetDisplayAttribute(value);
            return displayAttribute == null ? value.ToString() : displayAttribute.GetName();
        }

        public static string GetDescription(this Enum value)
        {
            var displayAttribute = GetDisplayAttribute(value);
            return displayAttribute == null ? "" : displayAttribute.GetDescription();
        }

        private static DisplayAttribute GetDisplayAttribute(Enum value)
        {
            var type = value.GetType();

            var members = type.GetMember(value.ToString());
            if (members.Length == 0) throw new ArgumentException($"error '{value}' not found in type '{type.Name}'");

            var member = members[0];
            var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length > 0)
            {
                return (DisplayAttribute)attributes[0];
            }

            return null;
        }

        public static T GetMaxValue<T>() where T : Enum
        {
            var enumType = typeof(T);

            var enumValues = enumType.GetEnumValues();
            var lastEnumValue = enumValues.GetValue(enumValues.Length - 1);
            var lastIntValue = (int)Convert.ChangeType(lastEnumValue, TypeCode.Int32);

            var max = (lastIntValue * 2) - 1;

            return (T)Enum.ToObject(enumType, max);
        }
    }
}
