using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class OrderStatusConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            OrderStatus result;
            Enum.TryParse<OrderStatus>(text, out result);
            if (result == default(OrderStatus))
            {
                return string.Empty;
            }
            else
            {
                return LookupHelper.GetOrderStatusName(result);
            }
        }

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            OrderStatus? data = null;
            if (value is int)
            {
                data = (OrderStatus?)(int)value;
            }
            if (value is Enum)
            {
                data = (OrderStatus?)value;
            }
            if (data.HasValue)
            {
                return LookupHelper.GetOrderStatusName(data.Value);
            }
            return string.Empty;
        }

        public override bool CanConvertFrom(Type type)
        {
            return type == typeof(OrderStatus);
        }

        public override bool CanConvertTo(Type type)
        {
            return true;
        }
    }
}
