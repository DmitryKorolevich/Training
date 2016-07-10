using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class ObjectHistoryLogListItemModel
    {
        public string DataReferenceId { get; set; }
        public int IdObject { get; set; }

        public int IdObjectStatus { get; set; }

        public ObjectType IdObjectType { get; set; }

        public DateTime DateCreated { get; set; }

        public string EditedBy { get; set; }

        public string OptionalData { get; set; }

        public string Data { get; set; }

        public ObjectHistoryLogListItemModel(ObjectHistoryLogItem item)
        {
            if (item != null)
            {
                IdObject = item.IdObject;
                IdObjectStatus = item.IdObjectStatus;
                IdObjectType = (ObjectType) item.IdObjectType;
                DateCreated = item.DateCreated;
                EditedBy = item.EditedBy;
                OptionalData = item.OptionalData;
                if (item.DataItem != null)
                {
                    Data = item.DataItem.Data;
                }
                DataReferenceId = item.IdObjectHistoryLogDataItem?.ToString();
            }
        }

        public ObjectHistoryLogListItemModel(ObjectHistoryItem item)
        {
            if (item != null)
            {
                IdObject = item.IdObject;
                IdObjectStatus = item.IdObjectStatus;
                IdObjectType = (ObjectType)item.IdObjectType;
                DateCreated = item.DateCreated;
                EditedBy = item.EditedBy;
                OptionalData = item.OptionalData;
                DataReferenceId = item.DataReferenceId;
            }
        }

        public ObjectHistoryLogListItemModel(LogDataItemTableEntity item)
        {
            if (item != null)
            {
                IdObject = int.Parse(item.PartitionKey);
                IdObjectStatus = item.IdObjectStatus;
                IdObjectType = (ObjectType) item.IdObjectType;
                DateCreated = item.DateCreated;
                EditedBy = item.EditedBy;
                OptionalData = item.OptionalData;
                Data = item.Data;
                DataReferenceId = item.PartitionKey + "_" + item.RowKey;
            }
        }
    }
}