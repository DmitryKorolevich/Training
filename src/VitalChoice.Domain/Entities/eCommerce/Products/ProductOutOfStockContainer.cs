using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class ProductOutOfStockContainer : Entity
    {
        public int IdProduct { get; set; }

        public string Name { get; set; }

        public bool InStock { get; set; }

        public ICollection<ProductOutOfStockRequest> Requests { get; set; }
    }
}