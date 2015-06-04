using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public class Lookup : Entity
    {
        public string LookupValueType { get; set; }

        public ICollection<LookupVariant> LookupVariants { get; set; }
    }
}
