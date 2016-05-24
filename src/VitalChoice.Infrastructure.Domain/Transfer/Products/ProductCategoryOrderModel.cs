using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class ProductCategoryOrderModel
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public CustomerTypeCode? IdVisibility { get; set; }
	}
}
