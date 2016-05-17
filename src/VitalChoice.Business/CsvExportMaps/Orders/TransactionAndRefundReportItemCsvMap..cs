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
            Map(m => m.IdObjectType).Name("O Type").Index(5).TypeConverter<OrderTypeConverter>();
            Map(m => m.RefundIdRedeemType).Name("Redeem").Index(6).TypeConverter<RefundRedeemConverter>();
            Map(m => m.Code).Name("SKU").Index(7);
            Map(m => m.DisplayName).Name("Description").Index(8);
            Map(m => m.OrderQuantity).Name("Quantity").Index(9);
            Map(m => m.ProductIdObjectType).Name("Product Type").Index(10).TypeConverter<ShortProductTypeConverter>();
            Map(m => m.IdCustomer).Name("Customer #").Index(11);
            Map(m => m.CustomerLastName).Name("Last Name").Index(12);
            Map(m => m.CustomerFirstName).Name("First Name").Index(13);
            Map(m => m.CustomerIdObjectType).Name("Customer Type").Index(14).TypeConverter<CustomerTypeConverter>();
            Map(m => m.PaymentMethodIdObjectType).Name("Payment Method").Index(15).TypeConverter<PaymentMethodTypeConverter>();
            Map(m => m.Price).Name("Price").Index(16).TypeConverterOption("c");
            Map(m => m.RefundProductPercent).Name("Product %").Index(17).TypeConverter<PercentConverter>();
            Map(m => m.DiscountIdObjectType).Name("Discount Type").Index(18).TypeConverter<DiscountTypeConverter>();
            Map(m => m.DiscountPercent).Name("Discount %").Index(19).TypeConverter<PercentConverter>();
            Map(m => m.DiscountedSubtotal).Name("Discounted Subtotal").Index(20).TypeConverterOption("c");
            Map(m => m.Override).Name("Override").Index(21);
            Map(m => m.ServiceCodeName).Name("Service Code").Index(22);
            Map(m => m.ReturnAssociated).Name("Return").Index(23);
            Map(m => m.ShippingTotal).Name("Shipping").Index(24).TypeConverterOption("c");
            Map(m => m.TaxTotal).Name("Tax").Index(25).TypeConverterOption("c");
            Map(m => m.Total).Name("Total").Index(26).TypeConverterOption("c");
        }
    }
}
