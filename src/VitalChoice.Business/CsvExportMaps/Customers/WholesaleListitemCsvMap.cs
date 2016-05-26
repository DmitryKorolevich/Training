using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.CsvExportMaps.Customers
{
    public class WholesaleListitemCsvMap : CsvClassMap<WholesaleListitem>
    {
        public WholesaleListitemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Id).Name("Account ID").Index(0);
            Map(m => m.Company).Name("Company Name").Index(1);
            Map(m => m.Tier).Name("Tier").Index(2);
            Map(m => m.TradeClass).Name("Trade Class").Index(3);
            Map(m => m.SalesLastThreeMonths).Name("Sales (Last 90 days)").Index(4).TypeConverterOption("c");
            Map(m => m.SalesLastYear).Name("Sales (Last 12 months)").Index(5).TypeConverterOption("c");
            Map(m => m.LastOrderDate).Name("Last Sale Date").Index(6).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.DateCreated).Name("Inception Date").Index(7).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.FirstName).Name("Contact First Name").Index(8);
            Map(m => m.LastName).Name("Contact Last Name").Index(9);
            Map(m => m.Email).Name("Contact Email").Index(10);
            Map(m => m.Phone).Name("Contact Phone").Index(11);
        }
    }
}
