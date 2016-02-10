using System;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Products
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