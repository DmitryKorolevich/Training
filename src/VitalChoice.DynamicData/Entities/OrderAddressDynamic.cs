using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class OrderAddressDynamic: MappedObject
    {
        public int IdOrder { get; set; }

        public int IdCountry { get; set; }

        public string County { get; set; }

        public int? IdState { get; set; }
    }
}
