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

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class WholesaleDropShipReportOrderItemCsvMap : CsvClassMap<WholesaleDropShipReportOrderItem>
    {
        public WholesaleDropShipReportOrderItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.IdOrder).Name("Order ID").Index(0);
            Map(m => m.OrderStatus).Name("Status").Index(1).TypeConverter<OrderStatusConverter>();
            Map(m => m.POrderStatus).Name("P Status").Index(2).TypeConverter<OrderStatusConverter>();
            Map(m => m.NPOrderStatus).Name("NP Status").Index(3).TypeConverter<OrderStatusConverter>();
            Map(m => m.Total).Name("Total").Index(4).TypeConverterOption("c");
            Map(m => m.DiscountedSubtotal).Name("Discounted Subtotal").Index(5).TypeConverterOption("c");
            Map(m => m.Shipping).Name("Shipping").Index(6).TypeConverterOption("c");
            Map(m => m.OrderNotes).Name("Order Specific Notes").Index(7);
            Map(m => m.PoNumber).Name("PO#").Index(8);
            Map(m => m.ShippingCompany).Name("Profile Company Name").Index(9);
            Map(m => m.ShippingFirstName).Name("First Name").Index(10);
            Map(m => m.ShippingLastName).Name("Last Name").Index(11);
            Map(m => m.ShippingAddress1).Name("Address 1").Index(12);
            Map(m => m.ShippingAddress2).Name("Address 2").Index(13);
            Map(m => m.City).Name("City").Index(14);
            Map(m => m.StateCode).Name("State").Index(15);
            Map(m => m.Zip).Name("Postal Code").Index(16);
            Map(m => m.Country).Name("Country").Index(17);
            Map(m => m.Phone).Name("Phone").Index(18);
            Map(m => m.ShipDate).Name("Ship Date").Index(19).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.ShippingCarrier).Name("Shipping Carrier").Index(20);
            Map(m => m.ShippingIdConfirmation).Name("Shipping Confirmation #").Index(21);
            Map(m => m.PShipDate).Name("P Ship Date").Index(22).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.PShippingCarrier).Name("P Shipping Carrier").Index(23);
            Map(m => m.PShippingIdConfirmation).Name("P Shipping Confirmation #").Index(24);
            Map(m => m.NPShipDate).Name("NP Ship Date").Index(25).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.NPShippingCarrier).Name("NP Shipping Carrier").Index(26);
            Map(m => m.NPShippingIdConfirmation).Name("NP Shipping Confirmation #").Index(27);
            Map(m => m.Skus).Name("Products").Index(28).TypeConverter<SkusConverter>();
        }

        private class SkusConverter : DefaultTypeConverter
        {
            public override string ConvertToString(TypeConverterOptions options, object value)
            {
                var data = value as ICollection<WholesaleDropShipReportSkuItem>;
                return data != null ? string.Join(", ", data.Select(d => $"{d.Code} ({d.Quantity})")) : string.Empty;
            }

            public override bool CanConvertTo(Type type)
            {
                return true;
            }
        }
    }
}
