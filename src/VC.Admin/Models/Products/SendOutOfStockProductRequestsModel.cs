using System;
using System.Collections.Generic;
using VC.Admin.Validators.Product;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Entities.Promotion;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VC.Admin.Models.Product
{
    public class SendOutOfStockProductRequestsModel : BaseModel
    {
        public ICollection<int> Ids { get; set; }

        public string MessageFormat { get; set; }
    }
}