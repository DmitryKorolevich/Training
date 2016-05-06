using System;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Business.CsvExportMaps.Orders;

namespace VitalChoice.Business.CsvExportMaps
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
            Map(m => m.OrderStatus).Name("Order Status").Index(6).TypeConverter<OrderStatusConverter>();
            Map(m => m.Commission).Name("Commission").Index(7);
        }
    }
}
