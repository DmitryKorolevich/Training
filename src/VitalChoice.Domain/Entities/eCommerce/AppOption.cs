using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce
{
    public class AppOption : Entity
    {
        public string OptionName { get; set; }

        public string OptionValue { get; set; }
    }
}