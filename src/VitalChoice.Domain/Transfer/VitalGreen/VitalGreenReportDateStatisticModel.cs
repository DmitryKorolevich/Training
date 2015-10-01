using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.VitalGreen;

namespace VitalChoice.Domain.Transfer.VitalGreen
{
    public class VitalGreenReportDateStatisticModel
    {
        public DateTime Date { get; set; }

        public int CompletedCount { get; set; }

        public ICollection<VitalGreenRequest> Requests { get; set; }
    }
}
