using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class CustomerSkuUsageReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int? IdCustomerType { get; set; }

        public int? IdCategory { get; set; }

        public bool ExportRemoveEmailDublicates { get; set; }

        public IList<int> SkuIds { get; set; }

        public ICollection<KeyValuePair<int, int>> Exclude;
    }
}