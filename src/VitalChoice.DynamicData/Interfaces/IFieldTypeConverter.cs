using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IFieldTypeConverter
    {
        object ConvertFrom(object obj);
        object DefaultValue { get; }
    }
}