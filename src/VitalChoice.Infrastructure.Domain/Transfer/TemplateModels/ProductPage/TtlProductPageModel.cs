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
			Tabs = new List<TtlProductPageTabContent>();
	    }

	    public string Name { get; set; }

		public string Url { get; set; }

	    public string Image { get; set; }

	    public string ShortDescription { get; set; }

	    public string SpecialIconUrl { get; set; }

	    public IList<TtlRelatedYoutubeVideoModel> YoutubeVideos { get; set; }

	    public IList<TtlCrossSellProductModel> CrossSells { get; set; }

	    public IList<TtlBreadcrumbItemModel> BreadcrumbOrderedItems { get; set; }

	    public IList<TtlProductPageSkuModel> Skus { get; set; }

	    public IList<TtlProductPageTabContent> Tabs { get; set; }
    }
}
