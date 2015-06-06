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
using VitalChoice.Domain.Constants;

namespace VC.Admin.Models.Product
{
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

    public class SKUManageModel : Model<SkuDynamic, IMode>, IModelToDynamic<SkuDynamic>
    {
        public int? Id { get; set; }

        [Localized(GeneralFieldNames.SKU)]
        public string Name { get; set; }

        public bool Active { get; set; }

        public bool Hidden { get; set; }

        public decimal RetailPrice { get; set; }

        public decimal WholesalePrice { get; set; }

        [Map]
        public int? Stock { get; set; }

        [Map]
        public bool DisregardStock { get; set; }

        [Map]
        public bool DisallowSingle { get; set; }

        [Map]
        public bool NonDiscountable { get; set; }

        [Map]
        public bool OrphanType { get; set; }

        [Map]
        public bool AutoShipProduct { get; set; }

        [Map]
        public double OffPercent { get; set; }

        [Map]
        public int Seller { get; set; }

        [Map]
        public bool HideFromDataFeed { get; set; }

        [Map]
        public bool AutoShipFrequency1 { get; set; }

        [Map]
        public bool AutoShipFrequency2 { get; set; }

        [Map]
        public bool AutoShipFrequency3 { get; set; }

        [Map]
        public bool AutoShipFrequency6 { get; set; }

        public SKUManageModel()
        {
        }

        public override SkuDynamic Convert()
        {
            SkuDynamic toReturn = new SkuDynamic();

            return toReturn;
        }

        public void FillDynamic(SkuDynamic dynamicObject)
        {
        }

        public void FillSelfFrom(SkuDynamic dynamicObject)
        {
            Id = dynamicObject.Id;
            Name = dynamicObject.Code;
            Active = dynamicObject.StatusCode==RecordStatusCode.Active;
            Hidden = dynamicObject.Hidden;
            RetailPrice = dynamicObject.Price;
            WholesalePrice = dynamicObject.WholesalePrice;
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
        public string MainProductImage { get; set; }

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
            //if (CrossSellProducts != null && CrossSellProducts.Count > 3)
            //{
            //    dynamicObject.Data.CrossSellProduct1 = CrossSellProducts[0];
            //    dynamicObject.Data.CrossSellProduct2 = CrossSellProducts[1];
            //    dynamicObject.Data.CrossSellProduct3 = CrossSellProducts[2];
            //    dynamicObject.Data.CrossSellProduct4 = CrossSellProducts[3];
            //}
        }

        public void FillSelfFrom(ProductDynamic dynamicObject)
        {
            Id = dynamicObject.Id;
            Name = dynamicObject.Name;
            Url = dynamicObject.Url;
            Type = dynamicObject.Type;
            StatusCode = dynamicObject.StatusCode;
            Hidden = dynamicObject.Hidden;
            CategoryIds = dynamicObject.CategoryIds.ToList();

            CrossSellProducts = new List<CrossSellProductModel>()
                    {
                        new CrossSellProductModel(),
                        new CrossSellProductModel(),
                        new CrossSellProductModel(),
                        new CrossSellProductModel(),
                    };
            for (int i = 1; i <= ProductConstants.FIELD_COUNT_CROSS_SELL_PRODUCT; i++)
            {
                var crossSellProduct = CrossSellProducts[i];
                if (dynamicObject.DictionaryData.ContainsKey(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_IMAGE + i))
                {
                    crossSellProduct.Image = (string)dynamicObject.DictionaryData[ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_IMAGE + i];
                }
                if (dynamicObject.DictionaryData.ContainsKey(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_URL + i))
                {
                    crossSellProduct.Url = (string)dynamicObject.DictionaryData[ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_URL + i];
                }
            }

            Videos = new List<VideoModel>()
                    {
                        new VideoModel(),
                        new VideoModel(),
                        new VideoModel(),
                    };
            for (int i = 1; i <= ProductConstants.FIELD_COUNT_YOUTUBE; i++)
            {
                var video = Videos[i];
                if (dynamicObject.DictionaryData.ContainsKey(ProductConstants.FIELD_NAME_YOUTUBE_IMAGE + i))
                {
                    video.Image = (string)dynamicObject.DictionaryData[ProductConstants.FIELD_NAME_YOUTUBE_IMAGE + i];
                }
                if (dynamicObject.DictionaryData.ContainsKey(ProductConstants.FIELD_NAME_YOUTUBE_TEXT + i))
                {
                    video.Text = (string)dynamicObject.DictionaryData[ProductConstants.FIELD_NAME_YOUTUBE_TEXT + i];
                }
                if (dynamicObject.DictionaryData.ContainsKey(ProductConstants.FIELD_NAME_YOUTUBE_VIDEO + i))
                {
                    video.Video = (string)dynamicObject.DictionaryData[ProductConstants.FIELD_NAME_YOUTUBE_VIDEO + i];
                }
            }
        }
    }
}