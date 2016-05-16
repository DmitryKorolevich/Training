using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Business.CsvExportMaps
{
    public class ShortProductTypeConverter : DefaultTypeConverter
    {
        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            ProductType? data = (ProductType?)value;
            if (data.HasValue)
            {
                return LookupHelper.GetShortProductTypeName(data.Value);
            }
            return string.Empty;
        }

        public override bool CanConvertTo(Type type)
        {
            return true;
        }
    }
}
