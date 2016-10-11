using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Business.CsvExportMaps
{
    public class YesNoRecordStatusConverter : DefaultTypeConverter
    {
        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            RecordStatusCode? data = null;
            if (value is int)
            {
                data = (RecordStatusCode?)(int)value;
            }
            if (value is Enum)
            {
                data = (RecordStatusCode?)value;
            }
            if (data.HasValue)
            {
                return LookupHelper.GetYesNoRecordStatus(data.Value);
            }
            return string.Empty;
        }

        public override bool CanConvertFrom(Type type)
        {
            return type == typeof(RecordStatusCode);
        }

        public override bool CanConvertTo(Type type)
        {
            return true;
        }
    }
}
