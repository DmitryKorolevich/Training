using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ServiceCodeItemsFilter
    {
        public int ServiceCode { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public Paging Paging { get; set; }
    }
}
