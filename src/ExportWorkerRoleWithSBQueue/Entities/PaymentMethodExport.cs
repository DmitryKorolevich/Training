using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;

namespace ExportWorkerRoleWithSBQueue.Entities
{
    public class PaymentMethodExport : Entity
    {
        public byte[] CreditCardNumber { get; set; }
    }
}