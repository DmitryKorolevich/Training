using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public class BigStringValue : Entity
    {
        public long IdBigString { get; set; }

        public string Value { get; set; }
    }
}
