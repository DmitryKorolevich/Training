using System.Collections.Generic;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class ProductCategory : Entity
    {
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public ProductCategory Parent { get; set; }

        public ICollection<ProductCategory> SubCategories { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public ICollection<ProductToCategory> ProductToCategories { get; set; }

        public int Order { get; set; }
    }
}