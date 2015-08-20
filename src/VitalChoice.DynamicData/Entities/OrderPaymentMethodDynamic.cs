using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class OrderPaymentMethodDynamic : MappedObject
    {
        public OrderAddressDynamic Address { get; set; }

        public int IdOrder { get; set; }
    }
}
