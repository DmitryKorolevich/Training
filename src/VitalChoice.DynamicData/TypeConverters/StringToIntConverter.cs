using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.DynamicData.TypeConverters
{
    public class StringToIntConverter : IFieldTypeConverter
    {
        public object ConvertFrom(object obj)
        {
            string value = obj as string;
            if (value != null)
            {
                int result;
                return int.TryParse(value, out result) ? result : DefaultValue;
            }
            return obj as int? ?? DefaultValue;
        }

        public object DefaultValue => 0;
    }
}