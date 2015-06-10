﻿using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Products;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VC.Admin.Models.Product
{
    public class ProductListItemModel : Model<VProductSku, IMode>
    {
        public int? Id { get; set; }

        public int ProductId { get; set; }

        public string Thumbnail { get; set; }

        public string Name { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Hidden { get; set; }

        public ProductType IdProductType { get; set; }

        public ProductListItemModel(VProductSku item)
        {
            if(item!=null)
            {
                Id = item.SkuId;
                ProductId = item.IdProduct;
                Name = item.Name;
                Thumbnail = item.Thumbnail;
                StatusCode = item.StatusCode;
                Hidden = item.Hidden;
                IdProductType = item.IdProductType;
            }
        }
    }
}