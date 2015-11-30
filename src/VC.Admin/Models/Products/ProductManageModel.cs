using System.Collections.Generic;
using VC.Admin.Validators.Product;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Models.Product
{
    public class CrossSellProductModel
    {
        public string Image { get; set; }

        public string Url { get; set; }

        public bool IsDefault { get; set; }

        public CrossSellProductModel()
        {
        }
    }

    public class VideoModel
    {
        public string Image { get; set; }

        public string Video { get; set; }

        public string Text { get; set; }

        public bool IsDefault { get; set; }

        public VideoModel()
        {
        }
    }

    [ApiValidator(typeof(ProductManageModelValidator))]
    public class ProductManageModel : BaseModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

		[Map]
		public string SubTitle { get; set; }

		[Localized(GeneralFieldNames.Url)]
        public string Url { get; set; }

        public int MasterContentItemId { get; set; }

        [Map("IdObjectType")]
        public ProductType Type { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
        public bool Hidden { get; set; }

        [Map]
        public string Description { get; set; }

        [Map]
        public string DescriptionTitleOverride { get; set; }

        [Map]
        public bool DescriptionHide { get; set; }

        [Map]
        public string Serving { get; set; }

        [Map]
        public string ServingTitleOverride { get; set; }

        [Map]
        public bool ServingHide { get; set; }

        [Map]
        public string Recipes { get; set; }

        [Map]
        public string RecipesTitleOverride { get; set; }

        [Map]
        public bool RecipesHide { get; set; }

        [Map]
        public string Ingredients { get; set; }

        [Map]
        public string IngredientsTitleOverride { get; set; }

        [Map]
        public bool IngredientsHide { get; set; }

        [Map]
        public string Shipping { get; set; }

        [Map]
        public string ShippingTitleOverride { get; set; }

        [Map]
        public bool ShippingHide { get; set; }

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
        public string GoogleFeedTitle { get; set; }

        [Map]
        public string GoogleFeedDescription { get; set; }

        [Map]
        public string TaxCode { get; set; }

        [Map]
        public int? SpecialIcon { get; set; }

        [Map]
        public string Thumbnail { get; set; }

        [Map]
        public string MainProductImage { get; set; }

        [Map]
        public string SubProductGroupName { get; set; }

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

        [Map]
        public IList<int> CategoryIds { get; set; }

        public IList<CrossSellProductModel> CrossSellProducts { get; set; }

        public IList<VideoModel> Videos { get; set; }

        [Map("Skus")]
        public IList<SKUManageModel> SKUs { get; set; }

        [Map]
        public int? InventoryCategoryId { get; set; }

        public ProductManageModel()
        {
            SKUs = new List<SKUManageModel>();
            Videos = new List<VideoModel>();
            CrossSellProducts = new List<CrossSellProductModel>();
            CategoryIds = new List<int>();
        }

    }
}