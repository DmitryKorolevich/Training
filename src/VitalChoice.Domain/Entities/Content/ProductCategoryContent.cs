using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.Content
{
    public class ProductCategoryContent : Entity
    {
        //Stupid EF 7 think that any inheritance for him
        #region ProductCategory
        public string Name { get; set; }

        public string Url { get; set; }

        public int? ParentId { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public int Order { get; set; }

        #endregion

        public string FileImageSmallUrl { get; set; }

        public string FileImageLargeUrl { get; set; }

        public string LongDescription { get; set; }

        public string LongDescriptionBottom { get; set; }

        public string NavLabel { get; set; }

        public CustomerTypeCode? NavIdVisible { get; set; }

        public virtual MasterContentItem MasterContentItem { get; set; }

        public int MasterContentItemId { get; set; }

        public virtual ContentItem ContentItem { get; set; }

        public int ContentItemId { get; set; }

        public void Set(ProductCategory data)
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