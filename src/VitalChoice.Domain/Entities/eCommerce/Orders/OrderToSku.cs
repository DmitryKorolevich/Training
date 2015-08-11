using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.eCommerce.Orders
{
    public class OrderToSku : Entity
    {
        public int IdSku { get; set; }
        public Sku Sku { get; set; }
        public int IdOrder { get; set; }
        public Order Order { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
    }
}