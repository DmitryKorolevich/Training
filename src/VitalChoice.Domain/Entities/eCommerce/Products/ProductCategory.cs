﻿using System.Collections.Generic;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class ProductCategory : Entity
    {
        public string Name { get; set; }

        public string Url { get; set; }
        
        public int? ParentId { get; set; }

        public ProductCategory Parent { get; set; }

        public ICollection<ProductCategory> SubCategories { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public int Order { get; set; }

        public ICollection<ProductToCategory> ProductToCategories { get; set; }

        public ProductCategory()
        {
        }

        public ProductCategory(ProductCategoryContent data)
        {
            if (data != null)
            {
                Id = data.Id;
                Name = data.Name;
                Url = data.Url;
                ParentId = data.ParentId;
                StatusCode = data.StatusCode;
                Assigned = data.Assigned;
                Order = data.Order;
            }
        }
    }
}