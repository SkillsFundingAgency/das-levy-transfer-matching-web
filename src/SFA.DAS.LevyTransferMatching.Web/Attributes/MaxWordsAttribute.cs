using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxWordsAttribute : ValidationAttribute
    {
        private readonly int _maxWords;

        public MaxWordsAttribute(int maxWords)
        {
            _maxWords = maxWords;
        }

        public override bool IsValid(object value)
        {
            if (!(value is string str) || str.Length < 0)
            {
                return true;
            }

            return str.Replace("\r\n", " ").Split(" ").Count() <= _maxWords;
        }
    }
}
