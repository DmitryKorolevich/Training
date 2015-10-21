using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce.History
{
    public class ObjectHistoryLogItem : Entity
    {
        public long IdObjectHistoryLogItem { get; set; }

        public ObjectType IdObjectType { get; set; }

        public int IdObject { get; set; }

        public int IdObjectStatus { get; set; }

        public DateTime DateCreated { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedBy { get; set; }

        public long? IdObjectHistoryLogDataItem { get; set; }

        public ObjectHistoryLogDataItem DataItem { get; set; }

        public object LogObject { get; set; }

        public string OptionalData { get; set; }
    }
}
