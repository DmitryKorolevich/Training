using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Payment
{
    public class OrderPaymentMethodOptionValue : OptionValue<OrderPaymentMethodOptionType>
    {
        public int IdOrderPaymentMethod { get; set; }
    }
}
