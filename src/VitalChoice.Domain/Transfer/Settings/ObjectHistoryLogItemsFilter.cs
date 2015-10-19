using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Settings
{
    public class ObjectHistoryLogItemsFilter : FilterBase
    {
        public int IdObject { get; set; }

        public ObjectType IdObjectType { get; set; }

        public long? IdBeforeObjectHistoryLogItem { get; set; }
    }
}
