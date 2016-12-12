using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Business.CsvExportMaps
{
    public class RecordStatusCodeConverter : DefaultTypeConverter
    {
        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            RecordStatusCode? data = (RecordStatusCode?)(int?)value;
            if (data.HasValue)
            {
                return LookupHelper.GetRecordStatus(data.Value);
            }

            return string.Empty;
        }

        public override bool CanConvertTo(Type type)
        {
            return true;
        }
    }
}
