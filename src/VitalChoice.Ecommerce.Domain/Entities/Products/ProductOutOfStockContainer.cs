using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class ProductOutOfStockContainer : Entity
    {
        public int IdProduct { get; set; }

        public string Name { get; set; }

        public bool InStock { get; set; }

        public ICollection<ProductOutOfStockRequest> Requests { get; set; }
    }
}