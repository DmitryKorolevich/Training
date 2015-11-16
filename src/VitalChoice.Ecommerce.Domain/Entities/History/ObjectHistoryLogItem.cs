using System;

namespace VitalChoice.Ecommerce.Domain.Entities.History
{
    public class ObjectHistoryLogItem : Entity
    {
        public long IdObjectHistoryLogItem { get; set; }

        public int IdObjectType { get; set; }

        public int IdObject { get; set; }

        public int IdObjectStatus { get; set; }

        public DateTime DateCreated { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedBy { get; set; }

        public long? IdObjectHistoryLogDataItem { get; set; }

        public ObjectHistoryLogDataItem DataItem { get; set; }

        public string OptionalData { get; set; }
    }
}
