using System.Runtime.Serialization;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
    [DataContract]
    public class CustomerCardData
    {
        [DataMember]
        public int IdCustomer { get; set; }

        [DataMember]
        public int IdPaymentMethod { get; set; }

        [DataMember]
        public string CardNumber { get; set; }
    }

    [DataContract]
    public class OrderCardData
    {
        [DataMember]
        public int IdOrder { get; set; }

        [DataMember]
        public int? IdCustomerPaymentMethod { get; set; }

        [DataMember]
        public string CardNumber { get; set; }
    }
}