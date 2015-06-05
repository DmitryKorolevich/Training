using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Business.Entities;
using VitalChoice.DynamicData;
using VitalChoice.DynamicData.Attributes;

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

        public int? Stock { get; set; }

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
    public class ProductManageModel : Model<ProductDynamic, IMode>, IModelToDynamic<ProductDynamic>
    {
        public int? Id { get; set; }
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }
        [Localized(GeneralFieldNames.Url)]
        public string Url { get; set; }

        public ProductType Type { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Hidden { get; set; }

        [Map]
        public string Description { get; set; }

        [Map]
        public string Serving { get; set; }

        [Map]
        public string Recepies { get; set; }

        [Map]
        public string Ingredients { get; set; }

        [Map]
        public string ShortDescription { get; set; }

        [Map]
        public string ProductNotes { get; set; }

        [Map]
        public string MetaTitle { get; set; }

        [Map]
        public string MetaDescription { get; set; }

        [Map]
        public int? GoogleCategory { get; set; }

        [Map]
        public string TaxCode { get; set; }

        [Map]
        public int? SpecialIcon { get; set; }

        [Map]
        public string Thumbnail { get; set; }

        [Map]
        public string MainProductImage {get;set;}

        [Map]
        public string NutritionalTitle { get; set; }

        [Map]
        public string ServingSize { get; set; }

        [Map]
        public string Servings { get; set; }

        [Map]
        public string Calories { get; set; }

        [Map]
        public string CaloriesFromFat { get; set; }

        [Map]
        public string TotalFat { get; set; }

        [Map]
        public string TotalFatPercent { get; set; }

        [Map]
        public string SaturatedFat { get; set; }

        [Map]
        public string SaturatedFatPercent { get; set; }

        [Map]
        public string TransFat { get; set; }

        [Map]
        public string TransFatPercent { get; set; }

        [Map]
        public string Cholesterol { get; set; }

        [Map]
        public string CholesterolPercent { get; set; }

        [Map]
        public string Sodium { get; set; }

        [Map]
        public string SodiumPercent { get; set; }

        [Map]
        public string TotalCarbohydrate { get; set; }

        [Map]
        public string TotalCarbohydratePercent { get; set; }

        [Map]
        public string DietaryFiber { get; set; }

        [Map]
        public string DietaryFiberPercent { get; set; }

        [Map]
        public string Sugars { get; set; }

        [Map]
        public string SugarsPercent { get; set; }

        [Map]
        public string Protein { get; set; }

        [Map]
        public string ProteinPercent { get; set; }

        [Map]
        public string AdditionalNotes { get; set; }

        public IList<int> CategoryIds { get; set; }

        public IList<CrossSellProductModel> CrossSellProducts { get; set; }

        public IList<VideoModel> Videos { get; set; }

        [Map]
        public IList<SKUManageModel> SKUs { get; set; }

        public ProductManageModel()
        {
        }

        public ProductManageModel(ProductDynamic item)
        {
            //item.ToModel<ProductManageModel>(this);

            //SKUs = new List<SKUManageModel>();
            //if(item.SKUs!=null)
            //{
            //    SKUs = item.SKUs.Select(p => new SKUManageModel(p)).ToList();
            //}
        }

        public override ProductDynamic Convert()
        {
            ProductDynamic toReturn = new ProductDynamic();
            //toReturn.FromModel<ProductManageModel>(this);
            //toReturn.SKUs = new List<SKU>();
            //if (SKUs != null)
            //{
            //    toReturn.SKUs = SKUs.Select(p => p.Convert()).ToList();
            //}

            return toReturn;
        }

        public void FillDynamic(ProductDynamic dynamicObject)
        {
            if (CrossSellProducts != null && CrossSellProducts.Count > 3)
            {
                dynamicObject.Data.CrossSellProduct1 = CrossSellProducts[0];
                dynamicObject.Data.CrossSellProduct2 = CrossSellProducts[1];
                dynamicObject.Data.CrossSellProduct3 = CrossSellProducts[2];
                dynamicObject.Data.CrossSellProduct4 = CrossSellProducts[3];
            }
        }

        public void FillSelfFrom(ProductDynamic dynamicObject)
        {
            if (dynamicObject.DictionaryData.ContainsKey("CrossSellProduct1"))
            {
                CrossSellProducts = new List<CrossSellProductModel>(4)
                {
                    dynamicObject.Data.CrossSellProduct1,
                    dynamicObject.Data.CrossSellProduct2,
                    dynamicObject.Data.CrossSellProduct3,
                    dynamicObject.Data.CrossSellProduct4
                };
            }
        }
    }
}