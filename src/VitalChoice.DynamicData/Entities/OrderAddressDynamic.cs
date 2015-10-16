using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Attributes;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class OrderAddressDynamic: AddressDynamic
    {
        public int IdOrder { get; set; }
    }
}
