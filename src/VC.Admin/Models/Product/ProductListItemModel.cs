using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities;

namespace VC.Admin.Models.Product
{
    public class ProductListItemModel : Model<Product, IMode>
    {
        public int Id { get; set; }

        public string ThumbImage { get; set; }

        public string ProductName { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Hidden { get; set; }

        public ProductType Type { get; set; }

        public ProductListItemModel(Product item)
        {
            if(item!=null)
            {
                
            }
        }
    }
}