using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Product
{
    public class SKU
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<SKU> SKUs { get; set; }
    }

    public class SKUManageModel : Model<SKU, IMode>
    {
        public int Id { get; set; }

        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        public SKUManageModel()
        {
        }

        public SKUManageModel(SKU item)
        {
            Id = item.Id;
            Name = item.Name;
        }

        public override SKU Convert()
        {
            SKU toReturn = new SKU();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();

            return toReturn;
        }
    }

    [ApiValidator(typeof(ProductManageModelValidator))]
    public class ProductManageModel : Model<Product, IMode>
    {
        public int Id { get; set; }
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        public List<SKUManageModel> SKUs { get; set; }

        public ProductManageModel()
        {
        }

        public ProductManageModel(Product item)
        {
            Id = item.Id;
            Name = item.Name;
            SKUs = new List<SKUManageModel>();
            if(item.SKUs!=null)
            {
                SKUs = item.SKUs.Select(p => new SKUManageModel(p)).ToList();
            }
        }

        public override Product Convert()
        {
            Product toReturn = new Product();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.SKUs = new List<SKU>();
            if (SKUs != null)
            {
                toReturn.SKUs = SKUs.Select(p => p.Convert()).ToList();
            }

            return toReturn;
        }
    }
}