using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities
{
    public class LookupVariant : Entity
    {
        public int IdLookup { get; set; }

        public string ValueVariant { get; set; }
    }
}
