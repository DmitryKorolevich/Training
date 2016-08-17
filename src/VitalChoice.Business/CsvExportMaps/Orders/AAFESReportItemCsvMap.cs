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
    public class AAFESReportItemCsvMap : CsvClassMap<AAFESReportItem>
    {
        public AAFESReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.OrderNotes).Name("ORDER_NO").Index(0);
            Map(m => m.Code).Name("SKU_NO").Index(1);
            Map(m => m.Quantity).Name("QUANTITY").Index(2);
            Map(m => m.ShipMethodFreightCarrier).Name("SHIP_METHOD").Index(3);
            Map(m => m.TrackingNumber).Name("TRACKING_NO").Index(4);
            Map(m => m.IdOrder).Name("INVOICE_NO").Index(5);
        }
    }
}
