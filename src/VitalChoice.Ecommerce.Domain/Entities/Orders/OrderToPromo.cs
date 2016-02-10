using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderToPromo : Entity
    {
        public int IdSku { get; set; }
        public Sku Sku { get; set; }
        public int IdOrder { get; set; }
        public Order Order { get; set; }
        public int? IdPromo { get; set; }
        public Promotion Promo { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
    }
}