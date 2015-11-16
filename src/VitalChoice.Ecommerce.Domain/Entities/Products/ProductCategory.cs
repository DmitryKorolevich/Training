using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
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