using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class ProductCategoryOrderModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SubTitle { get; set; }

        public CustomerTypeCode? IdVisibility { get; set; }
    }
}
