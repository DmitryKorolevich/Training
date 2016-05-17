using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class OrderTypeConverter : DefaultTypeConverter
    {
        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            OrderType? data = (OrderType?)value;
            if (data.HasValue)
            {
                return LookupHelper.GetShortOrderTypeName(data.Value);
            }
            return string.Empty;
        }

        public override bool CanConvertTo(Type type)
        {
            return true;
        }
    }
}
