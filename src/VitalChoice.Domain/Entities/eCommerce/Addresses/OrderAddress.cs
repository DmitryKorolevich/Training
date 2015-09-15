using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Domain.Entities.eCommerce.Addresses
{
    public class OrderAddress : DynamicDataEntity<OrderAddressOptionValue, AddressOptionType>
    {
        public int IdCountry { get; set; }

        public string County { get; set; }

        public Country Сountry { get; set; }

        public int? IdState { get; set; }

        public State State { get; set; }
    }
}