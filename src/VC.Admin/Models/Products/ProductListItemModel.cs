using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Products
{
    public class ProductListItemModel : BaseModel
    {
        [Map("Id")]
        public int ProductId { get; set; }

        [Map]
        public string Thumbnail { get; set; }

        [Map]
        public string TaxCode { get; set; }

        [Map]
        public string Name { get; set; }

        [Map]
        public string SubTitle { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
        public CustomerTypeCode? IdVisibility { get; set; }

        [Map("IdObjectType")]
        public ProductType IdProductType { get; set; }

        [Map]
        public DateTime DateEdited { get; set; }

        public string EditedByAgentId { get; set; }

        [Map("Skus")]
        public IList<SkuListItemModel> SKUs { get; set; }

        public ProductListItemModel()
        {
            
        }

        public ProductListItemModel(VProductSku item)
        {
            if (item != null)
            {
                ProductId = item.IdProduct;
                Name = item.Name;
                SubTitle = item.SubTitle;
                Thumbnail = item.Thumbnail;
                TaxCode = item.TaxCode;
                StatusCode = item.StatusCode;
                IdVisibility = item.IdVisibility;
                IdProductType = item.IdProductType;
                DateEdited = item.DateEdited;
                EditedByAgentId = item.EditedByAgentId ?? String.Empty;
            }
        }

        public ProductListItemModel(ProductDynamic item)
        {
            if (item != null)
            {
                ProductId = item.Id;
                Name = item.Name;
                SubTitle = item.SafeData.SubTitle;
                Thumbnail = item.SafeData.Thumbnail;
                TaxCode = item.SafeData.TaxCode;
                StatusCode = (RecordStatusCode) item.StatusCode;
                IdVisibility = item.IdVisibility;
                IdProductType = (ProductType) item.IdObjectType;
                DateEdited = item.DateEdited;
                SKUs = item.Skus.Select(s => new SkuListItemModel(s, item)).ToList();
            }
        }
    }
}