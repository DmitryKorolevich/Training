using System;
using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;

namespace VitalChoice.Infrastructure.Domain.Transfer.VitalGreen
{
    public class VitalGreenReportDateStatisticModel
    {
        public DateTime Date { get; set; }

        public int CompletedCount { get; set; }

        public ICollection<VitalGreenRequest> Requests { get; set; }
    }
}
