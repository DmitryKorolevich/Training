using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce.History
{
    public class ObjectHistoryLogDataItem : Entity
    {
        public long IdObjectHistoryLogDataItem { get; set; }

        public string Data { get; set; }
    }
}
