using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.VitalGreen;
using VitalChoice.Domain.Transfer.Affiliates;
using VitalChoice.Domain.Transfer.Customers;

namespace VitalChoice.Business.ExportMaps
{
    public class AffiliateOrderListItemModelCsvMap : CsvClassMap<AffiliateOrderListItemModel>
    {
        public AffiliateOrderListItemModelCsvMap()
        {
            Map(m => m.IdOrder).Name("Order Id").Index(0);
            Map(m => m.DateCreated).Name("Order Date").Index(1);
            Map(m => m.CustomerName).Name("Customer Name").Index(2);
            Map(m => m.IdCustomer).Name("Cust ID").Index(3);
            Map(m => m.ProductsSubtotal).Name("Net Product Total").Index(4);
            Map(m => m.RepeatInCustomer).Name("Repeat").Index(5);
            Map(m => m.OrderStatus).Name("Order Status").Index(6).TypeConverter<OrderStatusConvertert>();
            Map(m => m.Commission).Name("Commission").Index(7);
        }
    }

    public class OrderStatusConvertert : DefaultTypeConverter
    {
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            OrderStatus result;
            Enum.TryParse<OrderStatus>(text, out result);
            if (result == default(OrderStatus))
            {
                return String.Empty;
            }
            else
            {
                return LookupHelper.GetOrderStatusName(result);
            }
        }

        public override bool CanConvertFrom(Type type)
        {
            return type == typeof(OrderStatus);
        }
    }
}
