using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Transfer.TemplateModels.ProductPage
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

	    public string Name { get; set; }

	    public string SubTitle { get; set; }

		public string Url { get; set; }

	    public string Image { get; set; }

	    public string ShortDescription { get; set; }

	    public int? SpecialIcon { get; set; }

	    public IList<TtlRelatedYoutubeVideoModel> YoutubeVideos { get; set; }

	    public IList<TtlCrossSellProductModel> CrossSells { get; set; }

	    public IList<TtlBreadcrumbItemModel> BreadcrumbOrderedItems { get; set; }

	    public IList<TtlProductPageSkuModel> Skus { get; set; }

	    public TtlProductPageTabContent DescriptionTab { get; set; }

	    public TtlProductPageTabContent IngredientsTab { get; set; }

	    public TtlProductPageTabContent RecipesTab { get; set; }

	    public TtlProductPageTabContent ServingTab { get; set; }

	    public TtlProductPageTabContent ShippingTab { get; set; }
    }
}
