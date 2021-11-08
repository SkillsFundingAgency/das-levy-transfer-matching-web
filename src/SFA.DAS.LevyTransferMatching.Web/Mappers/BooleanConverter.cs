using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace SFA.DAS.LevyTransferMatching.Web.Mappers
{
    public class BooleanConverter : DefaultTypeConverter
    {
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Yes" : "No";
            }

            return base.ConvertToString(value, row, memberMapData);
        }
    }
}
