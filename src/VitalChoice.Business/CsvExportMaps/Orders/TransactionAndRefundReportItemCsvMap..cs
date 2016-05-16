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
    public class TransactionAndRefundReportItemCsvMap : CsvClassMap<TransactionAndRefundReportItem>
    {
        public TransactionAndRefundReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.IdOrder).Name("Order #").Index(0);
            Map(m => m.Rank).Name("Key").Index(1);
            Map(m => m.OrderStatus).Name("Status").Index(2).TypeConverter<OrderStatusConverter>();
            Map(m => m.POrderStatus).Name("P Status").Index(3).TypeConverter<OrderStatusConverter>();
            Map(m => m.NPOrderStatus).Name("NP Status").Index(4).TypeConverter<OrderStatusConverter>();
            Map(m => m.RefundIdRedeemType).Name("Redeem").Index(5).TypeConverter<RefundRedeemConverter>();
            Map(m => m.Code).Name("SKU").Index(6);
            Map(m => m.DisplayName).Name("Description").Index(7);
            Map(m => m.OrderQuantity).Name("Quantity").Index(8);
            Map(m => m.ProductIdObjectType).Name("Product Type").Index(8).TypeConverter<ShortProductTypeConverter>();
            Map(m => m.IdCustomer).Name("Customer #").Index(9);
            Map(m => m.CustomerLastName).Name("Last Name").Index(10);
            Map(m => m.CustomerFirstName).Name("First Name").Index(11);
            Map(m => m.CustomerIdObjectType).Name("Customer Type").Index(12).TypeConverter<CustomerTypeConverter>();
            Map(m => m.PaymentMethodIdObjectType).Name("Payment Method").Index(13).TypeConverter<PaymentMethodTypeConverter>();
            Map(m => m.Price).Name("Price").Index(14).TypeConverterOption("c");
            Map(m => m.RefundProductPercent).Name("Product %").Index(15).TypeConverterOption("p2");
            Map(m => m.DiscountIdObjectType).Name("Discount Type").Index(16).TypeConverter<DiscountTypeConverter>();
            Map(m => m.DiscountPercent).Name("Discount %").Index(17).TypeConverterOption("p2");
            Map(m => m.DiscountedSubtotal).Name("Discounted Subtotal").Index(18).TypeConverterOption("c");
            Map(m => m.Override).Name("Override").Index(19);
            Map(m => m.ServiceCode).Name("Service Code").Index(20);
            Map(m => m.ReturnAssociated).Name("Return").Index(21);
            Map(m => m.ShippingTotal).Name("Shipping").Index(22).TypeConverterOption("c");
            Map(m => m.TaxTotal).Name("Tax").Index(23).TypeConverterOption("c");
            Map(m => m.Total).Name("Total").Index(24).TypeConverterOption("c");
        }
    }
}
