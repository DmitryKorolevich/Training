using System;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class ObjectHistoryItem
    {
        public ObjectHistoryItem(ObjectHistoryLogItem entity)
        {
            IdObjectType = entity.IdObjectType;
            IdObject = entity.IdObject;
            IdObjectStatus = entity.IdObjectStatus;
            DateCreated = entity.DateCreated;
            EditedBy = entity.EditedBy;
            OptionalData = entity.OptionalData;
            DataReferenceId = entity.IdObjectHistoryLogDataItem?.ToString();
        }

        public ObjectHistoryItem(LogDataItemTableEntity entity)
        {
            IdObjectType = entity.IdObjectType;
            IdObject = int.Parse(entity.PartitionKey);
            IdObjectStatus = entity.IdObjectStatus;
            DateCreated = entity.DateCreated;
            EditedBy = entity.EditedBy;
            OptionalData = entity.OptionalData;
            DataReferenceId = entity.PartitionKey + "_" + entity.RowKey;
        }

        public string DataReferenceId { get; set; }

        public int IdObjectType { get; set; }

        public int IdObject { get; set; }

        public int IdObjectStatus { get; set; }

        public DateTime DateCreated { get; set; }

        public string EditedBy { get; set; }

        public string OptionalData { get; set; }
    }
}