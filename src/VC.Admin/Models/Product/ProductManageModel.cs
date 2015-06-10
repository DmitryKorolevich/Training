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
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.DynamicData.Entities;

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

    [ApiValidator(typeof(ProductManageModelValidator))]
    public class ProductManageModel : Model<ProductDynamic, IMode>, IModelToDynamic<ProductDynamic>
    {
        [Map]
        public int Id { get; set; }

        [Map]
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        [Map]
        [Localized(GeneralFieldNames.Url)]
        public string Url { get; set; }

        [Map]
        public ProductType Type { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
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
            toReturn.FromModel<ProductManageModel, ProductDynamic>(this);

            return toReturn;
        }

        public void FillDynamic(ProductDynamic dynamicObject)
        {
            dynamicObject.CategoryIds = CategoryIds.ToList();

            if(CrossSellProducts!=null)
            {
                for(int i=0;i<CrossSellProducts.Count;i++)
                {
                    dynamicObject.DictionaryData.Add(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_IMAGE + (i + 1), CrossSellProducts[i].Image);
                    dynamicObject.DictionaryData.Add(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_URL + (i + 1), CrossSellProducts[i].Url);
                }
            }

            if (Videos != null)
            {
                for (int i = 0; i < Videos.Count; i++)
                {
                    dynamicObject.DictionaryData.Add(ProductConstants.FIELD_NAME_YOUTUBE_IMAGE + (i + 1), Videos[i].Image);
                    dynamicObject.DictionaryData.Add(ProductConstants.FIELD_NAME_YOUTUBE_TEXT + (i + 1), Videos[i].Text);
                    dynamicObject.DictionaryData.Add(ProductConstants.FIELD_NAME_YOUTUBE_VIDEO + (i + 1), Videos[i].Video);
                }
            }
        }

        public void FillSelfFrom(ProductDynamic dynamicObject)
        {
            CategoryIds = dynamicObject.CategoryIds.ToList();
            if(SKUs==null)
            {
                SKUs = new List<SKUManageModel>();
            }

            CrossSellProducts = new List<CrossSellProductModel>()
                    {
                        new CrossSellProductModel(),
                        new CrossSellProductModel(),
                        new CrossSellProductModel(),
                        new CrossSellProductModel(),
                    };
            for (int i = 1; i <= ProductConstants.FIELD_COUNT_CROSS_SELL_PRODUCT; i++)
            {
                var crossSellProduct = CrossSellProducts[i-1];
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
                var video = Videos[i-1];
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