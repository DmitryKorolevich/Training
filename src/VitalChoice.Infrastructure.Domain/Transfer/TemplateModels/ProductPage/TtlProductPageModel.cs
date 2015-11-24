using System;
using System.Collections.Generic;

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

	    public Guid ProductPublicId { get; set; }

	    public string Name { get; set; }

	    public string SubTitle { get; set; }

		public string Url { get; set; }

	    public string Image { get; set; }

	    public string ShortDescription { get; set; }

	    public int? SpecialIcon { get; set; }

	    public string SubProductGroupName { get; set; }

	    public IList<TtlRelatedYoutubeVideoModel> YoutubeVideos { get; set; }

	    public IList<TtlCrossSellProductModel> CrossSells { get; set; }

	    public IList<TtlBreadcrumbItemModel> BreadcrumbOrderedItems { get; set; }

	    public IList<TtlProductPageSkuModel> Skus { get; set; }

	    public TtlProductPageTabContentModel DescriptionTab { get; set; }

	    public TtlProductPageTabContentModel IngredientsTab { get; set; }

	    public TtlProductPageTabContentModel RecipesTab { get; set; }

	    public TtlProductPageTabContentModel ServingTab { get; set; }

	    public TtlProductPageTabContentModel ShippingTab { get; set; }

	    public TtlProductReviewsTabModel ReviewsTab { get; set; }
    }
}
