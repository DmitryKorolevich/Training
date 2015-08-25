using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Localization.Groups;
using VC.Admin.Validators.Order;

namespace VC.Admin.Models.Order
{
    [ApiValidator(typeof(OrderManageModelValidator))]
    public class OrderManageModel : BaseModel
    {
        [Map]
        public int Id { get; set; }

        public OrderManageModel()
        {
        }

    }
}