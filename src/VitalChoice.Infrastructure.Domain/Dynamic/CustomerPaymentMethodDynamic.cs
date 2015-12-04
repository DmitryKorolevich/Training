﻿using System.Runtime.Serialization;
using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public class CustomerPaymentMethodDynamic : MappedObject
    {
        [DataMember]
        public AddressDynamic Address { get; set; }

        [DataMember]
        public int IdCustomer { get; set; }
    }
}