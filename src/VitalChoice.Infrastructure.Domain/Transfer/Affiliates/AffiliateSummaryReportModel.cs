using System;

namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class AffiliateSummaryReportModel
    {
        public DateTime From { get; set; }

        public int IdType { get; set; }

        public int Count { get; set; }

        public decimal Sum { get; set; }
    }
}
