using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce.Affiliates
{
    public class AffiliateSummaryReportModel
    {
        public DateTime From { get; set; }

        public int IdType { get; set; }

        public int Count { get; set; }

        public decimal Sum { get; set; }
    }
}
