using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts
{
    [DataContract]
    public class GiftListExportModel
    {
        [DataMember]
        public int IdCustomer { get; set; }

        [DataMember]
        public int IdPaymentMethod { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Agent { get; set; }

        [DataMember]
        public string CustomerFirstName { get; set; }

        [DataMember]
        public string CustomerLastName { get; set; }

        [DataMember]
        public int ImportedOrdersCount { get; set; }

        [DataMember]
        public decimal ImportedOrdersAmount { get; set; }

        [DataMember]
        public ICollection<int> OrderIds { get; set; }
    }
}
