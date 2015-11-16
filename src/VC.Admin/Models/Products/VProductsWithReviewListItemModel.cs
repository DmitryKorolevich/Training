using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VC.Admin.Models.Product
{
    public class VProductsWithReviewListItemModel : BaseModel
    {
        public int IdProduct { get; set; }

        public string ProductName { get; set; }

        public int Count { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal Rating { get; set; }

        public VProductsWithReviewListItemModel(VProductsWithReview item)
        {
            if(item!=null)
            {
                IdProduct = item.IdProduct;
                ProductName = item.ProductName;
                Count = item.Count;
                DateCreated = item.DateCreated;
                Rating = item.Rating;
            }
        }
    }
}