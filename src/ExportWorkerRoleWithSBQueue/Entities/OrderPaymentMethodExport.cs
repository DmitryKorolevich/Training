using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExportWorkerRoleWithSBQueue.Entities
{
    public class OrderPaymentMethodExport : PaymentMethodExport
    {
        public int IdOrder { get; set; }
    }
}