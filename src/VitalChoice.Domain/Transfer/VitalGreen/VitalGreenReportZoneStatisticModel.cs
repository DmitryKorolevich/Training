using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.VitalGreen;

namespace VitalChoice.Domain.Transfer.VitalGreen
{
    public class VitalGreenReportZoneStatisticModel
    {
        public int CompletedCount { get; set; }

        public FedExZone Zone { get; set; }
    }
}
