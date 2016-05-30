using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Entities.Products
{
    public class SkuBreakDownReportRawItem : Entity
    {
        public int IdSku { get; set; }

        public int CustomerIdObjectType { get; set; }

        public int IdProduct { get; set; }

        public string Code { get; set; }

        public decimal Amount { get; set; }

        public int Quantity { get; set; }
    }
}
