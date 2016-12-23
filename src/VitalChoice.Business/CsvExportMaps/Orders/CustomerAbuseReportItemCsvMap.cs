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
    public class CustomerAbuseReportItemCsvMap : CsvClassMap<CustomerAbuseReportItem>
    {
        public CustomerAbuseReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.IdCustomer).Name("Customer #").Index(0);
            Map(m => m.CustomerFirstName).Name("Customer First Name").Index(1);
            Map(m => m.CustomerLastName).Name("Customer Last Name").Index(2);
            Map(m => m.TotalOrders).Name("Total Orders").Index(3);
            Map(m => m.LastOrderDateCreated).Name("Last Order Date").Index(4).
                TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.TotalReships).Name("Total Reships").Index(5);
            Map(m => m.TotalRefunds).Name("Total Refunds").Index(6);
            Map(m => m.ServiceCodes).Name("Service Codes").Index(7);
        }
    }
}
