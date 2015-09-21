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
    public class ProductOutOfStockContainerListItemModel : BaseModel
    {
        public int IdProduct { get; set; }

        public string Name { get; set; }

        public ICollection<ProductOutOfStockRequest> Requests { get; set; }

        public ProductOutOfStockContainerListItemModel(ProductOutOfStockContainer item)
        {
            if(item!=null)
            {
                IdProduct = item.IdProduct;
                Name = item.Name;
                Requests = item.Requests;
            }
        }
    }
}