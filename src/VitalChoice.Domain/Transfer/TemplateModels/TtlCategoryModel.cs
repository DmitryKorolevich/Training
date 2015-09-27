using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Transfer.Products;

namespace VitalChoice.Domain.Transfer.TemplateModels
{
    public class TtlCategoryModel
    {
	    public TtlCategoryModel()
	    {
			SubCategories = new List<TtlCategoryModel>();
			SideMenuItems = new List<TtlSidebarMenuItemModel>();
			Products = new List<TtlCategoryProductModel>();
			BreadcrumbOrderedItems = new List<TtlCategoryBreadcrumbItemModel>();
        }

	    public string Name { get; set; }

		public string Url { get; set; }

		public int Order { get; set; }

		public string FileImageSmallUrl { get; set; }

		public string FileImageLargeUrl { get; set; }

		public string LongDescription { get; set; }

		public string LongDescriptionBottom { get; set; }

	    public IList<TtlCategoryModel> SubCategories { get; set; }

	    public IList<TtlCategoryProductModel> Products { get; set; }

	    public IList<TtlSidebarMenuItemModel> SideMenuItems { get; set; }

	    public IList<TtlCategoryBreadcrumbItemModel> BreadcrumbOrderedItems { get; set; }
    }
}
