using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.VitalGreen
{
    public class VitalGreenReportModel
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int CompletedCount { get; set; }

        public ICollection<VitalGreenReportZoneStatisticModel> Zones { get; set; }

        public ICollection<VitalGreenReportDateStatisticModel> Dates { get; set; }
    }
}
