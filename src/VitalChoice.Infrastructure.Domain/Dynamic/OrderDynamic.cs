using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public sealed class OrderDynamic : MappedObject
    {
        public OrderDynamic()
        {
            Skus = new List<SkuOrdered>();
            PromoSkus = new List<PromoOrdered>();
            GiftCertificates = new List<GiftCertificateInOrder>();
        }

        public int? IdAddedBy { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public OrderStatus? POrderStatus { get; set; }

        public OrderStatus? NPOrderStatus { get; set; }

        //[NotLoggedInfo]
        public CustomerDynamic Customer { get; set; }

        public ICollection<SkuOrdered> Skus { get; set; }

        public DiscountDynamic Discount { get; set; }

        public ICollection<GiftCertificateInOrder> GiftCertificates { get; set; }

        public OrderPaymentMethodDynamic PaymentMethod { get; set; }

        public AddressDynamic ShippingAddress { get; set; }

        public decimal Total { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public int? IdOrderSource { get; set; }

        public AffiliateOrderPayment AffiliateOrderPayment { get; set; }

        public bool IsFirstHealthwise { get; set; }

        public ICollection<PromoOrdered> PromoSkus { get; set; }

        //Don't storing in DB, should be set be calculating logic
        public decimal? AffiliatePaymentAmount { get; set; }

        //Don't storing in DB, should be set be calculating logic
        public bool? AffiliateNewCustomerOrder { get; set; }

        public ICollection<ReshipProblemSkuOrdered> ReshipProblemSkus { get; set; }

        //Don't storing in DB
        public int? SendSide { get; set; }
    }
}