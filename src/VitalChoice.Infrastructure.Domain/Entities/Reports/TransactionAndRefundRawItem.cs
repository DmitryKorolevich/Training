using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class TransactionAndRefundRawItem : Entity
    {
        public long RowNumber { get; set; }

        public int IdOrder { get; set; }

        public int TotalCount { get; set; }

        public int? IdOrderSource { get; set; }

        public int Rank { get; set; }

        public int IdObjectType { get; set; }

        public int? OrderStatus { get; set; }

        public int? POrderStatus { get; set; }

        public int? NPOrderStatus { get; set; }

        public int? ServiceCode { get; set; }

        public int IdCustomer { get; set; }

        public int CustomerIdObjectType { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public int? IdPaymentMethod { get; set; }

        public int? IdDiscount { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public bool? ReturnAssociated { get; set; }

        public int? PaymentMethodIdObjectType { get; set; }

        public int? DiscountIdObjectType { get; set; }

        public decimal? DiscountPercent { get; set; }

        public int IdSku { get; set; }

        public int IdProduct { get; set; }

        public string Code { get; set; }

        public string ProductName { get; set; }

        public string ProductSubTitle { get; set; }

        public int? SkuQTY { get; set; }

        public int OrderQuantity { get; set; }

        public int? ProductIdObjectType { get; set; }

        public decimal Price { get; set; }

        public int? RefundIdRedeemType { get; set; }

        public decimal? RefundProductPercent { get; set; }
    }
}
