using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VC.Admin.Models.Product
{
    [ApiValidator(typeof(ProductReviewManageModelValidator))]
    public class ProductReviewManageModel : BaseModel
    {
        public int Id { get; set; }

        public int IdProduct { get; set; }

        public string ProductName { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public string CustomerName { get; set; }

        [Localized(GeneralFieldNames.Email)]
        public string Email { get; set; }

        public string Title { get; set; }

        public int Rating { get; set; }

        public string Description { get; set; }

        public ProductReviewManageModel()
        {
        }

        public ProductReviewManageModel(ProductReview item)
        {
            Id = item.Id;
            IdProduct = item.IdProduct;
            ProductName = item.Product?.Name;
            StatusCode = item.StatusCode;
            CustomerName = item.CustomerName;
            Email = item.Email;
            Title = item.Title;
            Rating = item.Rating;
            Description = item.Description;
        }

        public ProductReview Convert()
        {
            ProductReview toReturn = new ProductReview();
            toReturn.Id = Id;
            toReturn.IdProduct = IdProduct;
            toReturn.StatusCode = StatusCode;
            toReturn.CustomerName = CustomerName?.Trim();
            toReturn.Email = Email?.Trim();
            toReturn.Title = Title?.Trim();
            toReturn.Rating = Rating;
            toReturn.Description = Description;

            return toReturn;
        }
    }
}