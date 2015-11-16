using System.Collections.Generic;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VC.Admin.Models.Product
{
    public class ProductOutOfStockContainerListItemModel : BaseModel
    {
        public int IdProduct { get; set; }

        public string Name { get; set; }

        public bool InStock { get; set; }

        public ICollection<ProductOutOfStockRequest> Requests { get; set; }

        public ProductOutOfStockContainerListItemModel(ProductOutOfStockContainer item)
        {
            if(item!=null)
            {
                IdProduct = item.IdProduct;
                Name = item.Name;
                InStock = item.InStock;
                Requests = item.Requests;
            }
        }
    }
}