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
            Map(m => m.ShipDate).Name("Ship Date").Index(19);
            Map(m => m.ShippingCarrier).Name("Shipping Carrier").Index(20);
            Map(m => m.ShippingIdConfirmation).Name("Shipping Confirmation #").Index(21);
            Map(m => m.Skus).Name("Products").Index(22).TypeConverter<SkusConverter>();
        }

        public class SkusConverter : DefaultTypeConverter
        {
            public override string ConvertToString(TypeConverterOptions options, object value)
            {
                var data = value != null ? ((ICollection<WholesaleDropShipReportSkuItem>)value).ToList() : null;
                var toReturn = string.Empty;
                if (data!=null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        toReturn += $"{data[i].Code} ({data[i].Quantity})";
                        if (i != data.Count - 1)
                        {
                            toReturn += ", ";
                        }
                    }
                }
                return toReturn;
            }

            public override bool CanConvertTo(Type type)
            {
                return true;
            }
        }
    }
}
