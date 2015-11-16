using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;

namespace VitalChoice.Infrastructure.Domain.Transfer.VitalGreen
{
    public class VitalGreenReportZoneStatisticModel
    {
        public int CompletedCount { get; set; }

        public FedExZone Zone { get; set; }
    }
}
