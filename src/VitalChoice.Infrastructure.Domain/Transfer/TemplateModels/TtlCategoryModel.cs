using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Content.Products;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels
{
    public class TtlCategoryModel
    {
	    public TtlCategoryModel()
	    {
			SubCategories = new List<TtlCategoryModel>();
			SideMenuItems = new List<TtlSidebarMenuItemModel>();
			Products = new List<TtlCategoryProductModel>();
            Skus = new List<TtlCategorySkuModel>();
            BreadcrumbOrderedItems = new List<TtlBreadcrumbItemModel>();
        }

	    public string Name { get; set; }

		public string Url { get; set; }

		public int Order { get; set; }

		public string FileImageSmallUrl { get; set; }

		public string FileImageLargeUrl { get; set; }

		public string LongDescription { get; set; }

        public bool HideLongDescription { get; set; }

        public string LongDescriptionBottom { get; set; }

        public bool HideLongDescriptionBottom { get; set; }

        public int ViewType { get; set; }

        public string PublicHost { get; set; }

        public IList<TtlCategoryModel> SubCategories { get; set; }

	    public IList<TtlCategoryProductModel> Products { get; set; }

        public IList<TtlCategorySkuModel> Skus { get; set; }

        public IList<TtlSidebarMenuItemModel> SideMenuItems { get; set; }

	    public IList<TtlBreadcrumbItemModel> BreadcrumbOrderedItems { get; set; }
    }
}
