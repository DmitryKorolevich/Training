using System;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Payment
{
    public class OrderPaymentMethod : DynamicDataEntity<OrderPaymentMethodOptionValue, CustomerPaymentMethodOptionType>
    {
        public int IdOrder { get; set; }

        public int? IdAddress { get; set; }

        public Address BillingAddress { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }
}