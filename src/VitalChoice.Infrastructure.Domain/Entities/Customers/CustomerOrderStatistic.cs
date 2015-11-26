using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Entities.Customers
{
    public class CustomerOrderStatistic
    { 
        public int IdCustomer { get; set; }

        public int TotalOrders { get; set; }

        public DateTime? FirstOrderPlaced { get; set; }

        public DateTime? LastOrderPlaced { get; set; }
    }
}
