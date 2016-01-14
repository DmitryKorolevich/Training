using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Checkout
{
    public class CartToSku : Entity
    {
        public int IdSku { get; set; }
        public Sku Sku { get; set; }
        public int IdCart { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
    }
}
