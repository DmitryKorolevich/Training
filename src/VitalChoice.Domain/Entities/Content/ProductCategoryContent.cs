using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.Content
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

        public string LongDescriptionBottom { get; set; }

        public string NavLabel { get; set; }

        public CustomerTypeCode? NavIdVisible { get; set; }

        public ProductCategory ProductCategory { get; set; }
    }
}