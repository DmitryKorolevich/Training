using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;

namespace VitalChoice.Business.CsvExportMaps
{
    public class PercentConverter : DefaultTypeConverter
    {
        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            decimal? data = (decimal?)value;
            if (data.HasValue)
            {
                if (data.Value > 1)
                {
                    data = data.Value/100;
                }
                return string.Format($"{data:p2}");
            }
            return string.Empty;
        }

        public override bool CanConvertTo(Type type)
        {
            return true;
        }
    }
}
