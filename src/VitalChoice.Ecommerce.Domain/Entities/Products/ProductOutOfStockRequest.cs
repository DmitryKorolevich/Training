using System;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class ProductOutOfStockRequest : Entity
    {
        public int IdProduct { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime DateCreated { get; set; }
    }
}