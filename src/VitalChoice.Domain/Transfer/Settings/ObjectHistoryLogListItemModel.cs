using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Domain.Entities.eCommerce;

namespace VitalChoice.Domain.Transfer.Settings
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