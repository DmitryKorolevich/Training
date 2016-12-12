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
    public class DateStatusConverter : DefaultTypeConverter
    {
        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            DateStatus? data = (DateStatus?)value;
            if (data.HasValue)
            {
                return Enum.GetName(typeof(DateStatus), data.Value);
            }

            return string.Empty;
        }

        public override bool CanConvertTo(Type type)
        {
            return true;
        }
    }
}
