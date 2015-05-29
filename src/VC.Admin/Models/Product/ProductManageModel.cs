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

    public class CrossSellProductModel
    {
        public string Image { get; set; }

        public string Url { get; set; }

        public CrossSellProductModel()
        {
        }
    }

    public class VideoModel
    {
        public string Image { get; set; }

        public string Video { get; set; }

        public string Text { get; set; }

        public VideoModel()
        {
        }
    }

    public class SKUManageModel : Model<SKU, IMode>
    {
        public int? Id { get; set; }

        [Localized(GeneralFieldNames.SKU)]
        public string Name { get; set; }

        public bool Active { get; set; }

        public bool Hidden { get; set; }


        public double RetailPrice { get; set; }

        public double WholesalePrice { get; set; }

        public int Stock { get; set; }

        public bool DisregardStock { get; set; }

        public bool DisallowSingle { get; set; }

        public bool NonDiscountable { get; set; }

        public bool OrphanType { get; set; }

        public bool AutoShipProduct { get; set; }

        public double OffPercent { get; set; }

        public int Seller { get; set; }

        public bool HideFromDataFeed {get;set; }

        public bool AutoShipFrequency1 { get; set; }

        public bool AutoShipFrequency2 { get; set; }

        public bool AutoShipFrequency3 { get; set; }

        public bool AutoShipFrequency6 { get; set; }

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
            toReturn.Id = Id.HasValue ? Id.Value : 0;
            toReturn.Name = Name?.Trim();

            return toReturn;
        }
    }

    [ApiValidator(typeof(ProductManageModelValidator))]
    public class ProductManageModel : Model<Product, IMode>
    {
        public int? Id { get; set; }
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }
        [Localized(GeneralFieldNames.Url)]
        public string Url { get; set; }

        public ProductType Type { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Hidden { get; set; }


        public string Description { get; set; }

        public string Serving { get; set; }

        public string Recepies { get; set; }

        public string Ingredients { get; set; }

        public string ShortDescription { get; set; }

        public string ProductNotes { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public int? GoogleCategory { get; set; }

        public string TaxCode { get; set; }

        public int? SpecialIcon { get; set; }


        public string Thumbnail { get; set; }

        public string MainProductImage {get;set;}


        public string NutritionalTitle { get; set; }

        public string ServingSize { get; set; }

        public string Servings { get; set; }

        public string Calories { get; set; }

        public string CaloriesFromFat { get; set; }

        public string TotalFat { get; set; }

        public string TotalFatPercent { get; set; }

        public string SaturatedFat { get; set; }

        public string SaturatedFatPercent { get; set; }

        public string TransFat { get; set; }

        public string TransFatPercent { get; set; }

        public string Cholesterol { get; set; }

        public string CholesterolPercent { get; set; }

        public string Sodium { get; set; }

        public string SodiumPercent { get; set; }

        public string TotalCarbohydrate { get; set; }

        public string TotalCarbohydratePercent { get; set; }

        public string DietaryFiber { get; set; }

        public string DietaryFiberPercent { get; set; }

        public string Sugars { get; set; }

        public string SugarsPercent { get; set; }

        public string Protein { get; set; }

        public string ProteinPercent { get; set; }

        public string AdditionalNotes { get; set; }


        public IList<int> CategoryIds { get; set; }

        public IList<CrossSellProductModel> CrossSellProducts { get; set; }

        public IList<VideoModel> Videos { get; set; }

        public IList<SKUManageModel> SKUs { get; set; }

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
            toReturn.Id = Id.HasValue ? Id.Value : 0;
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