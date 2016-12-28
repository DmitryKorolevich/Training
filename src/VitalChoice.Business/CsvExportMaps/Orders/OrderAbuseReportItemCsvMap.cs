using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Reports;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class OrderAbuseReportItemCsvMap : CsvClassMap<OrderAbuseReportRawItem>
    {
        public OrderAbuseReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.IdCustomer).Name("Customer #").Index(0);
            Map(m => m.CustomerFirstName).Name("Customer First Name").Index(1);
            Map(m => m.CustomerLastName).Name("Customer Last Name").Index(2);
            Map(m => m.IdOrderSource).Name("Original Order #").Index(3);
            Map(m => m.OrderSourceDateCreated).Name("Original Order Date").Index(4).
                TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.OrderSourceProductsSubtotal).Name("Original Order Total").Index(5).TypeConverterOption("c");
            Map(m => m.IdRefund).Name("Refund Order #").Index(6);
            Map(m => m.RefundDateCreated).Name("Refund Order Date").Index(7);
            Map(m => m.RefundTotal).Name("Refund Value").Index(8).TypeConverterOption("c");
            Map(m => m.IdReship).Name("Reship Order #").Index(9);
            Map(m => m.ReshipDateCreated).Name("Reship Date").Index(10);
            Map(m => m.OrderSourceAddedBy).Name("Original Agent ID").Index(11);
            Map(m => m.ServiceCodeName).Name("Service Code").Index(12);
            Map(m => m.ServiceCodeNotes).Name("Service Code Notes").Index(13);
        }
    }
}
