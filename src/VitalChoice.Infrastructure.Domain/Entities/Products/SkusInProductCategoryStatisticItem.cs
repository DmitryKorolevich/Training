using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Infrastructure.Domain.Entities.Products
{
    public class SkusInProductCategoryStatisticItem : Entity
    {
        public string Code { get; set; }

        public string ProductName { get; set; }

        public string Category { get; set; }

        public string ParentCategory { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }
    }
}
