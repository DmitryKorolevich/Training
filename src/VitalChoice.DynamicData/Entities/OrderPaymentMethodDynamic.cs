using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class OrderPaymentMethodDynamic : MappedObject
    {
        public AddressDynamic Address { get; set; }

        public int IdOrder { get; set; }
    }
}
