using System;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class ProductOutOfStockRequest : Entity
    {
        public int IdProduct { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime DateCreated { get; set; }
    }
}