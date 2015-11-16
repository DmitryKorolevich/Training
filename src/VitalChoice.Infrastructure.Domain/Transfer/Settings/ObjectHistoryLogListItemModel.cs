using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class ObjectHistoryLogListItemModel
    {
        public long Id { get; set; }

        public int IdObject { get; set; }

        public int IdObjectStatus { get; set; }

        public ObjectType IdObjectType { get; set; }

        public DateTime DateCreated { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedBy { get; set; }

        public string OptionalData { get; set; }

        public string Data { get; set; }

        public ObjectHistoryLogListItemModel(ObjectHistoryLogItem item)
        {
            if(item!=null)
            {
                Id = item.IdObjectHistoryLogItem;
                IdObject = item.IdObject;
                IdObjectStatus = item.IdObjectStatus;
                IdObjectType = (ObjectType)item.IdObjectType;
                DateCreated = item.DateCreated;
                IdEditedBy = item.IdEditedBy;
                EditedBy = item.EditedBy;
                OptionalData = item.OptionalData;
                if (item.DataItem != null)
                {
                    Data = item.DataItem.Data;
                }
            }
        }
    }
}