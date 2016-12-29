using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Healthwise;
using VitalChoice.Ecommerce.Domain.Entities.Payment;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderReviewRule : DynamicDataEntity<OrderReviewRuleOptionValue, OrderReviewRuleOptionType>
    {
        public int? IdAddedBy { get; set; }

        public string Name { get; set; }

        public ApplyType ApplyType { get; set; }
    }
}