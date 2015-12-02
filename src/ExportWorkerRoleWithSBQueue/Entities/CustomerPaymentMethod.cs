using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExportWorkerRoleWithSBQueue.Entities
{
    public class CustomerPaymentMethod : PaymentMethod
    {
        public int IdCustomer { get; set; }
        public int IdPaymentMethod { get; set; }
    }
}