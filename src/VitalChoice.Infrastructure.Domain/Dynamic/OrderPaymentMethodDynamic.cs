﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Dynamic.Masking;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    [DataContract]
    [MaskProperty("CardNumber", typeof(CreditCardMasker))]
    [JsonObject(MemberSerialization.OptOut)]
    public sealed class OrderPaymentMethodDynamic : MappedObject
    {
        public AddressDynamic Address { get; set; }

        [DataMember]
        public int IdOrder { get; set; }
    }
}