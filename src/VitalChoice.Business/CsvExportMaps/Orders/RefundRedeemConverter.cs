using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class RefundRedeemConverter : DefaultTypeConverter
    {
        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            RedeemType? data = (RedeemType?)value;
            if (data.HasValue)
            {
                return Enum.GetName(typeof(RedeemType),data.Value);
            }
            return string.Empty;
        }

        public override bool CanConvertTo(Type type)
        {
            return true;
        }
    }
}
