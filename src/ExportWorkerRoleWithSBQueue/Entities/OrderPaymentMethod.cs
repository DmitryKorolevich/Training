using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExportWorkerRoleWithSBQueue.Entities
{
    public class OrderPaymentMethod : PaymentMethod
    {
        public int IdOrder { get; set; }
    }
}