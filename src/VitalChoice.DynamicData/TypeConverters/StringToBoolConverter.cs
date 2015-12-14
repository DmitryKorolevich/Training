using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.TypeConverters
{
    public class StringToBoolConverter: IFieldTypeConverter
    {
        public object ConvertFrom(object obj)
        {
            string value = obj as string;
            if (value != null)
            {
                bool result;
                return bool.TryParse(value, out result) ? result : DefaultValue;
            }
            return obj as bool? ?? DefaultValue;
        }

        public object DefaultValue => false;
    }
}
