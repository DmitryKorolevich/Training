using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Entities.Base
{
    public class Lookup : Entity
    {
        public string LookupValueType { get; set; }

        public ICollection<LookupVariant> LookupVariants { get; set; }

		public string Name { get; set; }

        public string Description { get; set; }
    }
}
