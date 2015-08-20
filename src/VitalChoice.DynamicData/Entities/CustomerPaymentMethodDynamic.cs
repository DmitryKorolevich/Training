﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public class CustomerPaymentMethodDynamic : MappedObject
    {
        public CustomerAddressDynamic Address { get; set; }

        public int IdCustomer { get; set; }
    }
}