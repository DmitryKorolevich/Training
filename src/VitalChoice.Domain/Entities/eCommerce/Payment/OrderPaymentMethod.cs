using System;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Orders;

namespace VitalChoice.Domain.Entities.eCommerce.Payment
{
    public class OrderPaymentMethod : DynamicDataEntity<OrderPaymentMethodOptionValue, CustomerPaymentMethodOptionType>
    {
        public int? IdAddress { get; set; }

        public OrderAddress BillingAddress { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }
}