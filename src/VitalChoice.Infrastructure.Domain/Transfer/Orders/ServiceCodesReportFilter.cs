using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ServiceCodesReportFilter
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }
}
