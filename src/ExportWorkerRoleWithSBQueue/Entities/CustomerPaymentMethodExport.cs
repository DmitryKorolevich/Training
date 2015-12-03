using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExportWorkerRoleWithSBQueue.Entities
{
    public class CustomerPaymentMethodExport : PaymentMethodExport
    {
        public int IdCustomer { get; set; }
        public int IdPaymentMethod { get; set; }
    }
}