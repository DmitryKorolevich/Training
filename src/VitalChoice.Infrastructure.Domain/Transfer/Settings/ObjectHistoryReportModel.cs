namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class ObjectHistoryReportModel
    {
        public ObjectHistoryLogListItemModel Main { get; set; }

        public ObjectHistoryLogListItemModel Before { get; set; }

        public ObjectHistoryReportModel()
        {
        }
    }
}