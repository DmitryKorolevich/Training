using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrderExportError
    {
        public string Error { get; set; }

        public int Id { get; set; }
    }
}
