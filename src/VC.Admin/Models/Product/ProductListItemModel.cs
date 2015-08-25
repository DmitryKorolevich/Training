using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VC.Admin.Models.Product
{
    public class ProductListItemModel : BaseModel
    {
        public int? Id { get; set; }

        public int ProductId { get; set; }

        public string Thumbnail { get; set; }

        public string TaxCode { get; set; }

        public string Name { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Hidden { get; set; }

        public ProductType IdProductType { get; set; }

        public DateTime DateEdited { get; set; }

        public string EditedByAgentId {get;set;}

        public IList<SkuListItemModel> SKUs { get; set; }

        public ProductListItemModel(VProductSku item)
        {
            if(item!=null)
            {
                Id = item.SkuId;
                ProductId = item.IdProduct;
                Name = item.Name;
                Thumbnail = item.Thumbnail;
                TaxCode = item.TaxCode;
                StatusCode = item.StatusCode;
                Hidden = item.Hidden;
                IdProductType = item.IdProductType;
                DateEdited = item.DateEdited;
                EditedByAgentId = item.EditedByAgentId ?? String.Empty;
            }
        }
    }
}