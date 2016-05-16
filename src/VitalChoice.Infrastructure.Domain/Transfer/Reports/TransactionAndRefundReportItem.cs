using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class TransactionAndRefundReportItem
    {
        public int IdOrder { get; set; }

        public int? IdOrderSource { get; set; }

        public int Rank { get; set; }

        public OrderType IdObjectType { get; set; }

        public OrderType? OrderStatus { get; set; }

        public OrderType? POrderStatus { get; set; }

        public OrderType? NPOrderStatus { get; set; }

        public int? ServiceCode { get; set; }

        public int IdCustomer { get; set; }

        public CustomerTypeCode CustomerIdObjectType { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public int? IdPaymentMethod { get; set; }

        public int? IdDiscount { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public bool? ReturnAssociated { get; set; }

        public PaymentMethodType? PaymentMethodIdObjectType { get; set; }

        public DiscountType? DiscountIdObjectType { get; set; }

        public decimal? DiscountPercent { get; set; }

        public int IdSku { get; set; }

        public int IdProduct { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public int OrderQuantity { get; set; }

        public ProductType? ProductIdObjectType { get; set; }

        public decimal Price { get; set; }

        public RedeemType? RefundIdRedeemType { get; set; }

        public decimal? RefundProductPercent { get; set; }

        public string Override { get; set; }
    }
}