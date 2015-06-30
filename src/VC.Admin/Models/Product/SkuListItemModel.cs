using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VC.Admin.Models.Product
{
    public class SkuListItemModel : BaseModel
    {
        public int? Id { get; set; }

        public int ProductId { get; set; }

        public string Code { get; set; }

        public string ProductName { get; set; }

        public decimal? Price { get; set; }

        public decimal? WholesalePrice { get; set; }

        public ProductType ProductType { get; set; }

        public SkuListItemModel(VSku item)
        {
            if(item!=null)
            {
                Id = item.SkuId;
                ProductId = item.IdProduct;
                Code = item.Code;
                ProductName = item.Name;
                Price = item.Price;
                WholesalePrice = item.WholesalePrice;
                ProductType = item.IdProductType;
            }
        }
    }
}