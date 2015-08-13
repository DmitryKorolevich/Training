using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Addresses
{
    public class OrderAddressOptionValue : OptionValue<AddressOptionType>
    {
        public int IdOrderAddress { get; set; }
    }
}
