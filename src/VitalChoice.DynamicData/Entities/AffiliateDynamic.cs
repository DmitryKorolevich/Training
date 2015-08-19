using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class AffiliateDynamic : MappedObject
    {
        public AffiliateDynamic()
        {
        }

        public string Name { get; set; }

        public decimal MyAppBalance { get; set; }

        public decimal CommissionFirst { get; set; }

        public decimal CommissionAll { get; set; }

        public int IdCountry { get; set; }

        public int? IdState { get; set; }

        public string County { get; set; }
    }
}
