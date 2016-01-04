using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VitalChoice.Infrastructure.Domain.Transfer.Healthwise
{
    public class VHealthwisePeriodFilter : FilterBase
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public bool NotBilledOnly { get; set; }

        public int? IdCustomer { get; set; }

        public bool NotPaid { get; set; }
    }
}