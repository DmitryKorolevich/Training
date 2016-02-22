using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Infrastructure.Domain.Content.Products
{
    public class ProductCategoryContent: ContentDataItem
    {
        public ProductCategoryContent()
        {
            ProductCategory = new ProductCategory();
        }

        public string FileImageSmallUrl { get; set; }

        public string FileImageLargeUrl { get; set; }

        public string LongDescription { get; set; }

        public bool HideLongDescription { get; set; }

        public string LongDescriptionBottom { get; set; }

        public bool HideLongDescriptionBottom { get; set; }

        public string NavLabel { get; set; }
        
        public CustomerTypeCode? NavIdVisible { get; set; }

        [NotMapped]
        public ProductCategory ProductCategory { get; set; }

        [NotMapped]
        public ICollection<ProductNavCategoryLite> SubCategories { get; set; }
    }
}