using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities
{
    public class Lookup : Entity
    {
        public string LookupValueType { get; set; }

        public ICollection<LookupVariant> LookupVariants { get; set; }
    }
}
