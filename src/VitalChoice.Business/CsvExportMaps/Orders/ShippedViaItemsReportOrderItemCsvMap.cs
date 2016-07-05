using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Reports;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class ShippedViaItemsReportOrderItemCsvMap : CsvClassMap<ShippedViaReportRawOrderItem>
    {
        public ShippedViaItemsReportOrderItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Id).Name("Order #").Index(0);
            Map(m => m.ShippedDate).Name("Ship Date").Index(1).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.DateCreated).Name("Order Date").Index(2).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.ShipMethodFreightCarrier).Name("Carrier").Index(3);
            Map(m => m.ShipMethodFreightServiceName).Name("Service").Index(4);
            Map(m => m.WarehouseName).Name("Warehouse").Index(5);
            Map(m => m.StateCode).Name("Dest. State").Index(6);
            Map(m => m.ServiceCodeName).Name("Service Code").Index(7);
            Map(m => m.IdCustomer).Name("Customer #").Index(8);
            Map(m => m.FirstName).Name("First Name").Index(9);
            Map(m => m.LastName).Name("Last Name").Index(10);
            Map(m => m.Total).Name("Total").Index(11).TypeConverterOption("c");
        }
    }
}
