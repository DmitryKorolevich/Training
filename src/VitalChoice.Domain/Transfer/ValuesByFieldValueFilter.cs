using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer
{
    public class ValuesByFieldValueFilter : FilterBase
    {
        public string FieldName { get; set; }

        public ICollection<int> FieldIds { get; set; }

        public string FieldValue { get; set; }

        public int? IdReferencedObjectType { get; set; }
    }
}