using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer
{
    public class ValuesByFieldValueFilter : FilterBase
    {
        public string FieldName { get; set; }

        public ICollection<int> FieldIds { get; set; }

        public string FieldValue { get; set; }
        
        public int? IdReferencedObjectType { get; set; }
    }
}