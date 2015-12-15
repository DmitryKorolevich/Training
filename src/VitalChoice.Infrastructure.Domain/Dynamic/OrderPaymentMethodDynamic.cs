using System.Runtime.Serialization;
using Newtonsoft.Json;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Dynamic.Masking;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    [DataContract]
    [MaskProperty("CardNumber", typeof(CreditCardMasker))]
    [JsonObject(MemberSerialization.OptOut)]
    public sealed class OrderPaymentMethodDynamic : PaymentMethodDynamic
    {
        [DataMember]
        public int IdOrder { get; set; }
    }
}