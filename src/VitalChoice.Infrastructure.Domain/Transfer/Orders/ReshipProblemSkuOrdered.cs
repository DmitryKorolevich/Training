using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ReshipProblemSkuOrdered
    {
        public int IdOrder { get; set; }

        public string Code { get; set; }

        public int IdSku { get; set; }
    }
}