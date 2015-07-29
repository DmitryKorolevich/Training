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
    [ApiValidator(typeof(InventoryCategoryManageModelValidator))]
    public class InventoryCategoryManageModel : BaseModel
    {
        public int Id { get; set; }
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public InventoryCategoryManageModel()
        {
        }

        public InventoryCategoryManageModel(InventoryCategory item)
        {
            Id = item.Id;
            Name = item.Name;
            ParentId = item.ParentId;
        }

        public InventoryCategory Convert()
        {
            InventoryCategory toReturn = new InventoryCategory();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.ParentId = ParentId;

            return toReturn;
        }
    }
}