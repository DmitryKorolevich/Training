using System.Runtime.Serialization;

namespace VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts
{
    [DataContract]
    public class CustomerExportInfo
    {
        [DataMember]
        public int IdCustomer { get; set; }

        [DataMember]
        public int IdPaymentMethod { get; set; }
    }

    [DataContract]
    public class CustomerCardData
    {
        [DataMember]
        public int IdCustomer { get; set; }

        [DataMember]
        public int IdPaymentMethod { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string SecurityCode { get; set; }
    }

    [DataContract]
    public class OrderCardData
    {
        [DataMember]
        public int IdOrder { get; set; }

        [DataMember]
        public int? IdCustomerPaymentMethod { get; set; }

        [DataMember]
        public int? IdOrderSource { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string SecurityCode { get; set; }
    }
}