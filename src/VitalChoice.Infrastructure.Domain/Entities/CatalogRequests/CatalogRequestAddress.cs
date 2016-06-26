using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Infrastructure.Domain.Entities.CatalogRequests
{
    public class CatalogRequestAddress : DynamicDataEntity<CatalogRequestAddressOptionValue, AddressOptionType>
    {
        public int? IdCountry { get; set; }

        public string County { get; set; }

        public Country Сountry { get; set; }

        public int? IdState { get; set; }

        public State State { get; set; }
    }
}
