using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductPageModel
    {
	    public TtlProductPageModel()
	    {
			YoutubeVideos = new List<TtlRelatedYoutubeVideoModel>();
			CrossSells = new List<TtlCrossSellProductModel>();
			BreadcrumbOrderedItems = new List<TtlBreadcrumbItemModel>();
		    Skus = new List<TtlProductPageSkuModel>();
	    }

        [Map("PublicId")]
	    public Guid ProductPublicId { get; set; }

        [Map]
	    public string Name { get; set; }

        [Map]
        public string SubTitle { get; set; }

        [Map]
        public string Url { get; set; }

        [Map("MainProductImage")]
        public string Image { get; set; }

        [Map]
	    public string ShortDescription { get; set; }

        [Map]
        public int? SpecialIcon { get; set; }

        [Map]
	    public string SubProductGroupName { get; set; }

        public bool ShowDiscountMessage { get; set; }
        
        public IList<TtlRelatedYoutubeVideoModel> YoutubeVideos { get; set; }

	    public IList<TtlCrossSellProductModel> CrossSells { get; set; }

	    public IList<TtlBreadcrumbItemModel> BreadcrumbOrderedItems { get; set; }

	    public IList<TtlProductPageSkuModel> Skus { get; set; }

	    public TtlProductPageTabModel DescriptionTab { get; set; }

	    public TtlProductIngredientsTabModel IngredientsTab { get; set; }

	    public TtlProductRecipesTabModel RecipesTab { get; set; }

	    public TtlProductPageTabModel ServingTab { get; set; }

	    public TtlProductPageTabModel ShippingTab { get; set; }

	    public TtlProductReviewsTabModel ReviewsTab { get; set; }
    }
}
