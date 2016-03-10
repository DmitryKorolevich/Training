using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Entities.Products
{
    public class VTopProducts : Entity
    {
        public int IdSku { get; set; }
        public Sku Sku { get; set; }
        public int Count { get; set; }
        public int IdCustomer { get; set; }
    }
}
