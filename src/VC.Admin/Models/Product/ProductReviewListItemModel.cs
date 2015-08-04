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
    public class ProductReviewListItemModel : BaseModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string CustomerName { get; set; }

        public string ProductName { get; set; }

        public DateTime DateCreated { get; set; }

        public int Rating { get; set; }

        public ProductReviewListItemModel(ProductReview item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Title = item.Title;
                CustomerName = item.CustomerName;
                if (item.Product != null)
                {
                    ProductName = item.Product.Name;
                }
                DateCreated = item.DateCreated;
                Rating = item.Rating;
            }
        }
    }
}