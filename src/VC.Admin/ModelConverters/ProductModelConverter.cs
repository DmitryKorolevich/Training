using System.Collections.Generic;
using VC.Admin.Models.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Admin.ModelConverters
{
    public class ProductModelConverter : BaseModelConverter<ProductManageModel, ProductDynamic>
    {
        public override void DynamicToModel(ProductManageModel model, ProductDynamic dynamic)
        {
            model.CrossSellProducts = new List<CrossSellProductModel>()
            {
                new CrossSellProductModel(),
                new CrossSellProductModel(),
                new CrossSellProductModel(),
                new CrossSellProductModel(),
            };
            for (int i = 1; i <= ProductConstants.FIELD_COUNT_CROSS_SELL_PRODUCT; i++)
            {
                var crossSellProduct = model.CrossSellProducts[i - 1];
                if (dynamic.DictionaryData.ContainsKey(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_IMAGE + i))
                {
                    crossSellProduct.Image = (string)dynamic.DictionaryData[ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_IMAGE + i];
                }
                if (dynamic.DictionaryData.ContainsKey(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_URL + i))
                {
                    crossSellProduct.Url = (string)dynamic.DictionaryData[ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_URL + i];
                }
            }

            model.Videos = new List<VideoModel>()
            {
                new VideoModel(),
                new VideoModel(),
                new VideoModel(),
                new VideoModel()
            };
            for (int i = 1; i <= ProductConstants.FIELD_COUNT_YOUTUBE; i++)
            {
                var video = model.Videos[i - 1];
                if (dynamic.DictionaryData.ContainsKey(ProductConstants.FIELD_NAME_YOUTUBE_IMAGE + i))
                {
                    video.Image = (string)dynamic.DictionaryData[ProductConstants.FIELD_NAME_YOUTUBE_IMAGE + i];
                }
                if (dynamic.DictionaryData.ContainsKey(ProductConstants.FIELD_NAME_YOUTUBE_TEXT + i))
                {
                    video.Text = (string)dynamic.DictionaryData[ProductConstants.FIELD_NAME_YOUTUBE_TEXT + i];
                }
                if (dynamic.DictionaryData.ContainsKey(ProductConstants.FIELD_NAME_YOUTUBE_VIDEO + i))
                {
                    video.Video = (string)dynamic.DictionaryData[ProductConstants.FIELD_NAME_YOUTUBE_VIDEO + i];
                }
            }
        }

        public override void ModelToDynamic(ProductManageModel model, ProductDynamic dynamic)
        {
            if (model.CrossSellProducts != null)
            {
                for (int i = 0; i < model.CrossSellProducts.Count; i++)
                {
                    dynamic.DictionaryData.Add(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_IMAGE + (i + 1), model.CrossSellProducts[i].Image);
                    dynamic.DictionaryData.Add(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_URL + (i + 1), model.CrossSellProducts[i].Url);
                }
            }

            if (model.Videos != null)
            {
                for (int i = 0; i < model.Videos.Count; i++)
                {
                    dynamic.DictionaryData.Add(ProductConstants.FIELD_NAME_YOUTUBE_IMAGE + (i + 1), model.Videos[i].Image);
                    dynamic.DictionaryData.Add(ProductConstants.FIELD_NAME_YOUTUBE_TEXT + (i + 1), model.Videos[i].Text);
                    dynamic.DictionaryData.Add(ProductConstants.FIELD_NAME_YOUTUBE_VIDEO + (i + 1), model.Videos[i].Video);
                }
            }
        }
    }
}