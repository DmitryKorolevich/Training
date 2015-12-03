using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Infrastructure.Domain.Entities.Products
{
    public class ProductCategoryStatisticItem : Entity
    {
        public decimal Amount { get; set; }
    }
}
